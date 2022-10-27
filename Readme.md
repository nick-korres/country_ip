# Project Title

A simple api written with .net that exposes 2 endpoints 

## Description

The two endpoints are :
    * GET /countries/:ip 
        that reponds with info of the country the ip belongs to.
    * POST /countries/reports
        that responds with the available ips per country given an array of country codes.

The api also uses in-memmory cache to save responses for some time to speed up the requests 
and updates the info in the database by using an external source.

## Executing program

```
dotnet run
```
