# Demo of integration test in Azure pipeline using docker container
### To run the integration test project locally, make sure you have docker engine installed and run the following command:
* `docker run --name mymssqlserver -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=myPa55w0rd' -p 1433:1433 -e 'MSSQL_PID=Standard' -d mcr.microsoft.com/mssql/server:2019-latest`
* If you change any param values in the above command, then please update `appsettings.json` file as needed.
### To run the API locally, you will need to run the above command to create an instance of the MSSQL server database. 