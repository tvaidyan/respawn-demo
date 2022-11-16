#!/bin/bash

if ! [ -z ${RUN_LOCAL_DB} ]; then
    /opt/mssql-tools/bin/sqlcmd -S localhost -d master -U SA -P "${SA_PASSWORD}" -Q 'select 1'
else
    exit 0
fi
