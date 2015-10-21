TinyMapper - a quick object mapper for .Net
======================================================
[![Nuget downloads](http://img.shields.io/nuget/dt/tinymapper.svg)](https://www.nuget.org/packages/TinyMapper/)
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/TinyMapper/TinyMapper/blob/master/LICENSE)
[![GitHub license](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](http://www.firsttimersonly.com/)
[![Build status](https://ci.appveyor.com/api/projects/status/n36c2i10bjaj1f93?svg=true)](https://ci.appveyor.com/project/Behzadkhosravifar/tinymapper)


## Performance Comparison

![Performance Comparison](https://raw.githubusercontent.com/TinyMapper/TinyMapper/master/Source/Benchmark/DataSource/PrimitiveTypeMapping.jpg)


## Installation

Available on [nuget](https://www.nuget.org/packages/TinyMapper/)

	PM> Install-Package TinyMapper

## Getting Started

```csharp
TinyMapper.Bind<Person, PersonDto>();

var person = new Person
{
	Id = Guid.NewGuid(),
	FirstName = "John",
	LastName = "Doe"
};

var personDto = TinyMapper.Map<PersonDto>(person);
```

Ignore mapping source members and bind members with different names/types

```csharp
TinyMapper.Bind<Person, PersonDto>(config =>
{
	config.Ignore(x => x.Id);
	config.Ignore(x => x.Email);
	config.Bind(source => source.LastName, target => target.Surname);
	config.Bind(target => source.Emails, typeof(List<string>));
});

var person = new Person
{
	Id = Guid.NewGuid(),
	FirstName = "John",
	LastName = "Doe",
	Emails = new List<string>{"support@tinymapper.net", "MyEmail@tinymapper.net"}
};

var personDto = TinyMapper.Map<PersonDto>(person);
```

`TinyMapper` supports the following platforms:
* .Net 3.0+
* Mono

## What to read

 * [TinyMapper: yet another object to object mapper for .net](http://www.codeproject.com/Articles/886420/TinyMapper-yet-another-object-to-object-mapper-for)
 * [Complex mapping](https://github.com/TinyMapper/TinyMapper/wiki/Complex-mapping)
 * [How to create custom mapping](https://github.com/TinyMapper/TinyMapper/wiki/Custom-mapping)


 ## This Contribute Updates

 * Map DataRow to object
 * Map IDataRecord to object
 * Map DataTable and IDataReader to List<object>
 * Map to DynamicObject