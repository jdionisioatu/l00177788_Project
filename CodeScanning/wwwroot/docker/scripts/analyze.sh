#!/bin/bash

RED="\033[31m"
YELLOW="\033[33m"
GREEN="\033[32m"
RESET="\033[0m"

print_green() {
    echo -e "${GREEN}${1}${RESET}"
}

SRC=/opt/src
FORMAT="sarif-latest"
LANGUAGE=csharp
if [ -z $QS ]
then
    QS="$LANGUAGE-security-and-quality.qls"
fi

THREADS=5
if [ -z $THREADS ]
then
    THREADS=5
fi

OUTPUT="/opt/results"
if [ -z $OUTPUT ]
then
    OUTPUT="/opt/results"
fi
DB=$SRC/codeql-db

mkdir -p ${OUTPUT}
echo "----------------"
print_green " [+] Language: $LANGUAGE"
print_green " [+] Query-suites: $QS"
print_green " [+] Database: $DB"
print_green " [+] Source: $SRC"
print_green " [+] Output: $OUTPUT"
print_green " [+] Format: $FORMAT"
echo "----------------"

echo -e "==> Creating DB: codeql database create --language=$LANGUAGE $DB -s $SRC"
codeql database create --threads=$THREADS --language=$LANGUAGE $DB -s $SRC

echo -e "==> Start Scanning: codeql database analyze --format=$FORMAT --output=$OUTPUT/issues.$FORMAT $DB $QS"
codeql database analyze --threads=$THREADS --format=$FORMAT --output=$OUTPUT/issues.$FORMAT $DB $QS

curl -H "Authorization: Token ${defectdojotoken}" -include -F minimum_severity="Info" -F skip_duplicates=true -F verified=false -F close_old_findings=true -F active=true -F scan_type="SARIF" -F auto_create_context=true -F product_name="${repository_name}" -F engagement_name="${branch}" -F product_type_name=RD -F file=@${OUTPUT}/issues.${FORMAT} "${defectdojourl}/api/v2/import-scan/"