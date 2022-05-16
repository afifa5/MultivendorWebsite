using MultivendorWebViewer.ViewModels;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace MultivendorWebViewer.Components
{
    public interface ISpecificationIdentifiable
    {
        int Id { get; }
        string CategoryName { get; }
        string ClassName { get; }
        string Code { get; }
        string Description { get; }
        string FormattedValue { get; }
        bool HasImage { get; }
        //IImageIdentifiable Image { get; }
        bool IsHighlighted { get; }
        //bool IsPrintable { get; }
        bool IsVisible { get; }
        //float Order { get; }
        string PersistentIdentity { get; }
        //ISpecificationTypeIdentifiable Type { get; }
        //ISpecificationTypeCategoryIdentifiable TypeCategory { get; }
        string TypeName { get; }
        string Unit { get; }
        object Value { get; }
        //bool HasMode(long mode);
        long Mode { get; }
    }

    public interface ISpecificationTypeIdentifiable
    {
        //ISpecificationTypeCategoryIdentifiable Category { get; }
        //string Code { get; }
        ISet<string> Codes { get; }
        bool HasCodes { get; }
        int Id { get; }
        long Mode { get; }
        string Name { get; }
        string PersistentIdentity { get; }
        float SequenceNumber { get; }
        string Unit { get; }
        //bool HasCode(string code);
        //bool HasMode(long mode);
    }

    public interface ISpecificationTypeCategoryIdentifiable
    {
        string Code { get; }
        ISet<string> Codes { get; }
        bool HasCodes { get; }
        long Mode { get; }
        string Name { get; }
        string PersistentIdentity { get; }

        bool HasCode(string code);
        bool HasMode(long mode);
    }

    public interface ISpecificationIdentifiables
    {
        IEnumerable<ISpecificationIdentifiable> Specifications { get; }
    }
}