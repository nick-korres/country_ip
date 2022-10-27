FROM mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04

USER root

COPY init.sql init.sql
COPY import-data.sh import-data.sh
COPY entrypoint.sh entrypoint.sh

RUN chmod 755 entrypoint.sh

CMD /bin/bash ./entrypoint.sh