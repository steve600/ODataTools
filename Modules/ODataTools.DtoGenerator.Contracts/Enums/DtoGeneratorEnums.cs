using ODataTools.Core.MarkupExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.DtoGenerator.Contracts.Enums
{
    public enum EdmxVersion
    {
        V1,
        V2,
        V3,
        V4,
        None
    }

    public enum ODataServiceVersion
    {
        V1,
        V2,
        V3,
        V4,
        None
    }

    public enum DtoGeneratorMode
    {
        DtoGenerator,
        DataSvcUtil,
        None
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Languages
    {
        [Description("C#")]
        CSharp,
        [Description("Visual Basic")]
        VB
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DataSvcUtilVersions
    {
        [Description("1.0")]
        Version1,
        [Description("2.0")]
        Version2
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DataClassOptions
    {
        [Description("DTO")]
        DTO,
        [Description("INotifyPropertyChanged")]
        INotifyPropertyChanged,
        [Description("Proxy-Classes")]
        Proxy
    }
}
