# APitems-LOL
An Entry for the LOL API Contest

The project contains a general solution with 3 projects.


ApiDataRetriever
==============
An API Data retreival project which uses WebRequest and parses the response with dynamics (seriously this was an awful idea, I needed to parse the data, but it was awful transforming the data into an object then saving it to use the data later...).

The project contains.... basically a bunch of requests.

Data
==============
It was our first time using MySQL, but surprisingly the MySql dlls works almost identical to MsSql dlls, which we have more experience. 
So basically a disposable class with commands for stored procedures..

LolApItemWeb
==============
A Web Project for the Web View hosted on http://201.231.163.73:3000/ (Had a lot of troubles deploying to a free web host, so it's active in my computer, it may go a bit slow and might no be active 24/7, sorry about that :) ).

About the Web Project.
It's a web with statistics about the surveyed sample in games (only NA for 5.11 - 5.14). 
It fetches the statistics of the selected Rank, Role, and Champ selected and shows them in:

A Chart for the Average AP/MR in both Patches.

A Chart for the Lifetime of the items affected in AP rework (when it started appearing and when it dissapeared).

And a Table for item pickrate comparison per minute.

_____________________________
Creators Note:
Overall it was a really cool experience. It was really fun to do. First experience with loads of stuff, serious data retrieval, dynamics, Bootstrap, GoogleCharts, MySql.
