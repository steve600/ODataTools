This application is a collection of tools that can simplify the work with OData services. At the beginning it was quite difficult for me to get into this topic because there are different versions of OData and depending on the used OData version there are different tools that have to be used. The idea was to combine the different tools in one user interface.

## DTO-Generator ##

If you want to call an OData service from a .NET framework application you need corresponding data classes that reflect the entities of the service. Depending on the used OData version there are different utilities you can use to generate the client-side code:

- DataSvcUtil.exe ([https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx](https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx "https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx")) works for V1-V3 OData services
- OData v4 Client Code Generator ([https://visualstudiogallery.msdn.microsoft.com/9b786c0e-79d1-4a50-89a5-125e57475937](https://visualstudiogallery.msdn.microsoft.com/9b786c0e-79d1-4a50-89a5-125e57475937 "https://visualstudiogallery.msdn.microsoft.com/9b786c0e-79d1-4a50-89a5-125e57475937")) works only for OData V4 services (as the name suggests)

These utilities generate the client-side data service classes to access the OData service directly from a .NET Framework client application. In some cases you would prefer to use simple DTO classes and implement the service calls yourself. For these cases a DTO Generator was implemented. Whithin the DTO Generator there are currently the following options:

- DTO - generate simple DTO classes
- INotifiyPropertyChanged - generates a base class which implements the *INotifyPropertyChanged* interface and the corresponding data classes
- Proxy classes - this option generates a base class for the *INotifyPropertyChanged* stuff

Here are some details about each generation option:

**DTO**

This option generates simple DTO classes based on the metadata information of the appropriate service. For  the Northwind reference service ([http://services.odata.org/V3/Northwind/Northwind.svc/](http://services.odata.org/V3/Northwind/Northwind.svc/)) this looks as follows:

XXXXXXXXXXXXXXXXXXXX

**INotifyPropertyChanged**

As the name suggests this option generates model classes which implement the *INotifiyPropertyChanged* interface. In addition to the model classes a base class (BindableBase.cs) is generated where you can find the implementation of the *INotifyPropertyChanged* interface. For  the Northwind reference service ([http://services.odata.org/V3/Northwind/Northwind.svc/](http://services.odata.org/V3/Northwind/Northwind.svc/)) this looks as follows:

XXXXXXXXXXXXXXXXXX

**Proxy classes**

With this option four types of classes are generated:

- *BindableBase.cs* - base class which implements the *INotifyPropertyChanged* interface
- *DtoProxyBase.cs* - base class that acts as a proxy/wrapper for the generated DTO classes which provides simple functions for the change tracking. This class derives from the generated BindableBase class.
- *OutputFileDto.cs* - in this file you can find the generated DTO classes
- *OutputFileProxy.cs* - this file contains the proxy classes for the generated DTOs.

Here is a small example to illustrate the whole thing:

Suppose the service defines an entity called *Person* and the DTO Generator generates the following simple DTO class:

![Simple DTO](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_ProxyClasses_01.png)

In addition to the simple DTO class a corresponding proxy class is generated which has the following structure:

![Proxy class for the simple DTO](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_ProxyClasses_02.png)

So you can use the simple DTO classes for the service calls and the result of the service call can be wrapped in the appropriate proxy class which can be directly used for the data binding in a WPF dialog for example. It would be conceivable that the proxy classes fulfill other functions such as change tracking (already implemented), validation or other tasks. 

Here is a class diagram of the generated base classes:

![](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_ProxyClasses_03.png)

Whithin the application this looks as follows:

XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

## DataSvcUtil GUI ##

DataSvcUtil.exe is a command-line tool provided by WCF Data Services that consumes an Open Data Protocol (OData) feed and generates the client data service classes that are needed to access a data service from a .NET Framework client application. Here you can find more information about this utility: [https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx](https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx)

Here a simple GUI was implemented which calls the DataSvcUtil.exe command-line tool. The user interface looks like this:

XXXXXXXXXXXXXX

## Model Visualization ##



## Planned features ##

- Client code generator
- OData Explorer
- ...

## Known issues ##



## Used open source projects ##

- AvalonEdit - [https://github.com/icsharpcode/AvalonEdit](https://github.com/icsharpcode/AvalonEdit "https://github.com/icsharpcode/AvalonEdit")
- Dragablz - [https://github.com/ButchersBoy/Dragablz](https://github.com/ButchersBoy/Dragablz "https://github.com/ButchersBoy/Dragablz")
- GraphX - [https://github.com/panthernet/GraphX](https://github.com/panthernet/GraphX "https://github.com/panthernet/GraphX")
- MahApps.Metro - [https://github.com/MahApps/MahApps.Metro](https://github.com/MahApps/MahApps.Metro "https://github.com/MahApps/MahApps.Metro")
- PRISM - [https://github.com/PrismLibrary/Prism](https://github.com/PrismLibrary/Prism "https://github.com/PrismLibrary/Prism")
- Roslyn (.NET Compiler Platform) - [https://github.com/dotnet/roslyn](https://github.com/dotnet/roslyn "https://github.com/dotnet/roslyn")
- WPFLocalizationExtension - [https://github.com/SeriousM/WPFLocalizationExtension](https://github.com/SeriousM/WPFLocalizationExtension "https://github.com/SeriousM/WPFLocalizationExtension")

