create database domainInformation

use domainInformation

create table domainContactInfo(domainURL varchar(100) primary key not null,
						   domainCreateDate varchar(100),
						   contactStreetAddress varchar(100), 
						   contactCity varchar(100),
						   contactName varchar(100),
						   contactState varchar(100),
						   contactEmail varchar(100), 
						   contactPhone varchar(100))

						   delete from domainContactInfo
						   select * from domainContactInfo
						   drop table domainContactInfo
