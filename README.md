# acquizapi
Web API for [Math Exercise](https://github.com/alvachien/mathexercise.git), built on ASP.NET Core.

## Install
To install this Web API to your own server, please follow the steps below.


### Step 1. Clone or Download
You can clone this [repository](https://github.com/alvachien/achihapi.git) or download it.


### Step 2. Setup your own database.
You need setup your own database (SQL Server based), and run sql under folder 'src':
1. init.sql


### Step 3. Change the appsettings.json by adding the connection string:
The appsettings.json has been removed because it defines the connection strings to the database. This file is manadatory to make the API works. 

An example file look like following:
```javascript
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=XXX;Initial Catalog=XXX;Persist Security Info=True;User ID=XXX;Password=XXX;",
    "DebugConnection": "Server=XXX;Database=XXX;Integrated Security=SSPI;MultipleActiveResultSets=true"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```


### Step 4. Run this API
Deploy this Web API to IIS or other server.


## Tools
Though I using Visual Studio 2017, the project can be processed by any IDE which supports ASP.NET Core.


## Unit Test
This unit test project also included. You can run the unit test to ensure the code run successfully. 

# Author
**Alva Chien (Hongjun Qian) | 钱红俊**

A programmer, and a certificated Advanced Photographer.  
 
Contact me:

1. Via mail: alvachien@163.com. Or,
2. [Check my flickr](http://www.flickr.com/photos/alvachien). Or,
3. [Visit my website](http://www.alvachien.com)


# Licence
MIT
