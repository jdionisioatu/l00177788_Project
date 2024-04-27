FROM ubuntu:22.04 AS codeql_base

ARG giturl
ARG repository_name
ARG branch
ARG defectdojourl
ARG defectdojotoken
ENV env_environment $giturl
ENV env_environment $repository_name
ENV env_environment $branch
ENV env_environment $defectdojourl
ENV env_environment $defectdojotoken

# tzdata install needs to be non-interactive
ENV DEBIAN_FRONTEND=noninteractive

# create user, install/update basics and python
RUN apt-get update && \
	apt-get upgrade -y && \
	apt-get install -y --no-install-recommends \
	software-properties-common \
	nodejs \
	vim \
	curl \
	wget \
	git \
    jq \
    sudo \
	build-essential \
	unzip \
	apt-transport-https \
	python3.10 \
	python3-venv \
	python3-pip \
	python3-setuptools \
	python3-dev \
	python-is-python3 \
	gnupg \
	g++ \
	make \
	gcc \
	apt-utils \
	rsync \
	file \
	dos2unix \
	gettext && \
	apt-get clean && \
	ln -sf /usr/bin/python3.8 /usr/bin/python && \
	ln -sf /usr/bin/pip3 /usr/bin/pip 

RUN apt remove dotnet* aspnetcore* netstandard*

# Install .NET Core and Java for tools/builds
RUN cd /tmp && \
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update; \
    apt-get install -y default-jdk apt-transport-https && \
    apt-get update && \
    rm packages-microsoft-prod.deb
COPY 99microsoft-dotnet.pref /etc/apt/preferences.d/
RUN apt-get install -y dotnet-sdk-6.0 dotnet-sdk-7.0 dotnet-sdk-8.0

# Clone our setup and run scripts
RUN mkdir -p /usr/local/startup_scripts
COPY container /usr/local/startup_scripts/

RUN pip3 install -r /usr/local/startup_scripts/requirements.txt

# Install latest codeQL
ENV CODEQL_HOME /root/codeql-home

# record the latest version of the codeql-cli
RUN python3 /usr/local/startup_scripts/get-latest-codeql-version.py > /tmp/codeql_version
RUN mkdir -p \
    ${CODEQL_HOME}/codeql-repo \
    /opt/codeql

# get the latest codeql queries and record the HEAD
RUN git clone --depth 1 https://github.com/github/codeql ${CODEQL_HOME}/codeql-repo && \
    git --git-dir ${CODEQL_HOME}/codeql-repo/.git log --pretty=reference -1 > /opt/codeql/codeql-repo-last-commit

RUN CODEQL_VERSION=$(cat /tmp/codeql_version) && \
    wget -q https://github.com/github/codeql-cli-binaries/releases/download/${CODEQL_VERSION}/codeql-linux64.zip -O /tmp/codeql_linux.zip && \
    unzip /tmp/codeql_linux.zip -d ${CODEQL_HOME} && \
    rm /tmp/codeql_linux.zip

ENV PATH="${CODEQL_HOME}/codeql:${PATH}"
COPY scripts /root/scripts

# Pre-compile our queries to save time later
#RUN /root/scripts/compile-qs.sh

WORKDIR /opt/src
RUN git clone --branch ${branch} ${giturl}

WORKDIR /root
CMD /root/scripts/analyze.sh