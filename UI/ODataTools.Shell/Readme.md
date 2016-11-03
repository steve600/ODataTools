This application is a collection of tools that can simplify the work with OData services. At the beginning it was quite difficult for me to get into this topic because there are different versions of OData and depending on the used OData version there are different tools that have to be used. The idea was to combine the different tools in one user interface.

## DTO-Generator ##

If you want to call an OData service from a .NET framework application you need corresponding data classes that reflect the entities of the service. Depending on the used OData version there are different utilities you can use to generate the client-side code:

* DataSvcUtil.exe <a href="https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx" target="top">https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx</a> works for V1-V3 OData services
* OData v4 Client Code Generator <a href="https://visualstudiogallery.msdn.microsoft.com/9b786c0e-79d1-4a50-89a5-125e57475937" target="top">https://visualstudiogallery.msdn.microsoft.com/9b786c0e-79d1-4a50-89a5-125e57475937</a> works only for OData V4 services (as the name suggests)

These utilities generate the client-side data service classes to access the OData service directly from a .NET Framework client application. In some cases you would prefer to use simple DTO classes and implement the service calls yourself. For these cases a DTO Generator was implemented. Within the DTO Generator there are currently the following options:

* **DTO** - generate simple DTO classes
* **INotifiyPropertyChanged** - generates a base class which implements the *INotifyPropertyChanged* interface and the corresponding data classes
* **Proxy-classes** - this option generates a base class for the *INotifyPropertyChanged* stuff

Here are some details about each generation option:

**DTO**

This option generates simple DTO classes based on the metadata information of the appropriate service. For  the Northwind reference service <a href="http://services.odata.org/V3/Northwind/Northwind.svc/" target="top">http://services.odata.org/V3/Northwind/Northwind.svc/</a> this looks as follows:

![](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_DtoGenerator_01.png)

As you can see a single file is generated with simple DTO classes for each entity of the service.

**INotifyPropertyChanged**

As the name suggests this option generates model classes which implement the *INotifiyPropertyChanged* interface. In addition to the model classes a base class (BindableBase.cs) is generated where you can find the implementation of the *INotifyPropertyChanged* interface. For  the Northwind reference service <a href="http://services.odata.org/V3/Northwind/Northwind.svc/" target="top">http://services.odata.org/V3/Northwind/Northwind.svc/</a> this looks as follows:

![](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_DtoGenerator_02.png)

**Proxy-classes**

With this option four types of classes are generated:

* *BindableBase.cs* - base class which implements the *INotifyPropertyChanged* interface
* *DtoProxyBase.cs* - base class that acts as a proxy/wrapper for the generated DTO classes which provides functions for the change tracking. This class derives from the generated BindableBase class.
* *OutputFile.cs* - in this file you can find the generated (simple) DTO classes
* *OutputFileProxy.cs* - this file contains the proxy classes for the generated DTOs.

Here is a small example to illustrate the whole thing:

Suppose the service defines an entity called *Person* and the DTO Generator generates the following simple DTO class:

![Simple DTO](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_ProxyClasses_01.png)

In addition to the simple DTO class a corresponding proxy class is generated which has the following structure:

![Proxy class for the simple DTO](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_ProxyClasses_02.png)

The constructor of the generated proxy class expects the corresponding DTO. The proxy controls the accesses to the original object (DTO) and expands the original object by additional functions (*INotifyPropertyChanged*, ChangeTracking, Validation, ...) 

So you can use the simple DTO classes for the service calls and the result of the service call can be wrapped in the appropriate proxy class which then can be directly used for the data binding in a WPF dialog for example. It would be conceivable that the proxy classes fulfill other functions such as change tracking (already implemented), validation or other tasks. 

Here is a class diagram of the generated base classes:

![](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_ProxyClasses_03.png)

Within the application this looks as follows:

![](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_DtoGenerator_03.png)

TODO:

* Unit tests
* UI Validation (Generation should only be possible if all required fields are filled correctly)
* direct call of a service to determine the meta information
* ...

## DataSvcUtil GUI ##

DataSvcUtil.exe is a command-line tool provided by WCF Data Services that consumes an Open Data Protocol (OData) feed and generates the client data service classes that are needed to access a data service from a .NET Framework client application. Here you can find more information about this utility: <a href="https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx" target="top">https://msdn.microsoft.com/de-de/library/ee383989(v=vs.110).aspx</a>

Here a simple GUI was implemented which calls the DataSvcUtil.exe command-line tool. The user interface looks like this:

![](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_DataSvcUtilGui_01.png)

## Model Visualization ##

To get an overview of the entities of an OData service a diagram can be very helpful. So there is the possiblilty to visualize the model information:

![](http://csharp-blog.de/wp-content/uploads/2016/11/ODataTools_ModelVisualizer_01.png)

TODO:

* 
* Styling
* More information about the entities (Length, Nullable, ...)
* ...

## Planned features ##

* Client code generator
* OData Explorer
* ...

## Known issues ##

* The generator for V4 OData-Services does not work properly (enums, collections, nullable, ...)
* The model visualizer for V4 OData-Services does not work properly
* Some styling issues (tab control, ...)
* the generator has only been tested with the reference services
* Save/Restore application settings does not work properly
* ...

## Used open source projects ##

This application heavily leans several third party libraries without which this tool would not have been possible. Many thanks for the producers of these libraries:

* AvalonEdit - <a href="https://github.com/icsharpcode/AvalonEdit" target="top">https://github.com/icsharpcode/AvalonEdit</a>
* Dragablz - <a href="https://github.com/ButchersBoy/Dragablz" target="top">https://github.com/ButchersBoy/Dragablz</a>
* GraphX - <a href="https://github.com/panthernet/GraphX" target="top">https://github.com/panthernet/GraphX</a>
* MahApps.Metro - <a href="https://github.com/MahApps/MahApps.Metro" target="top">https://github.com/MahApps/MahApps.Metro</a>
* Microsoft OData Stack - <a href="https://github.com/OData/odata.net/" target="top">https://github.com/OData/odata.net/</a>
* PRISM - <a href="https://github.com/PrismLibrary/Prism" target="top">https://github.com/PrismLibrary/Prism</a>
* Roslyn (.NET Compiler Platform) - <a href="https://github.com/dotnet/roslyn" target="top">https://github.com/dotnet/roslyn</a>
* WPFLocalizationExtension - <a href="https://github.com/SeriousM/WPFLocalizationExtension" target="top">https://github.com/SeriousM/WPFLocalizationExtension</a>

## Warranty Disclaimer: No Warranty!

IN NO EVENT SHALL THE AUTHOR, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR REDISTRIBUTE THIS PROGRAM AND DOCUMENTATION, BE LIABLE FOR ANY COMMERCIAL, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM INCLUDING, BUT NOT LIMITED TO, LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR LOSSES SUSTAINED BY THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS, EVEN IF YOU OR OTHER PARTIES HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

