# Rolodex

An internal employee directory so all of our employees can search our database of 30,000 employees.
We want any HR representatives to be able to add/edit/delete employees.
A typical directory entry has the following information:

* Name
* Job Title
* Location
* Email
* Phone Number(s)

## Requirements
- LocalDB is installed named as localdb (https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver16#create-and-connect-to-a-named-instance)

## Local Setup

- Execute `.\setup` from powershell to install needed utilities.
- Execute `.\psake` from powershell to setup initial solution and run integration tests.