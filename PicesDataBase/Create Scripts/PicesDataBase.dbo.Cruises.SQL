
drop table  if exists Cruises;
create table  Cruises 
   (
	CruiseName   char(10)      not null,
	ShipName     varchar(50)   null,
	Description  varchar(1024) null,
	DateStart    date          null,
	DateEnd      date          null,
	Location     varchar(50)   null,
	Objective    varchar(50)   null,
	Principal    varchar(50)   null,
	ResearchOrg  varchar(50)   null,
	
	primary key  (CruiseName  asc)
   )
;


Insert into  Cruises  (CruiseName, ShipName, Description, DateStart, DateEnd, Location, Objective, Principal, ResearchOrg)
             values ("ETP2007", "R/V Seward Johnson", "Eastern Tropical Pacific", "2007-10-17", "2007-11-18", "Eastern Tropical Pacific", "", "", "");


Insert into  Cruises  (CruiseName, ShipName, Description, DateStart, DateEnd, Location, Objective, Principal, ResearchOrg)
             values ("ETP2008", "R/V Knorr", "Eastern Tropical Pacific", "2008-12-07", "2009-01-05", "Eastern Tropical Pacific", "", "", "");



Insert into  Cruises  (CruiseName, ShipName, Description, DateStart, DateEnd, Location, Objective, Principal, ResearchOrg)
             values ("GB10A", "Ship Name", "Eastern Tropical Pacific", "2010-05-26", "2010-05-31", "Eastern Tropical Pacific", "", "", "");
