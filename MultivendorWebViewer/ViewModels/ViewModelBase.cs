using MultivendorWebViewer;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MultivendorWebViewer.ViewModels
{
    public interface IViewModel 
    {
        ApplicationRequestContext ApplicationRequestContext { get; }
        ConfigurationManager Configuration { get; }

        UserSettings UserSettings { get; }
    }


    public abstract class ViewModelBase 
    {
        protected ViewModelBase()
        {
        }

        protected ViewModelBase(ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
        }

        public ApplicationRequestContext ApplicationRequestContext { get; set; }


        public MultivendorWebViewer.Manager.ConfigurationManager Configuration { get { return ApplicationRequestContext.Configuration; } }


        //protected string GetTextTranslation(Text text)
        //{
        //    return ApplicationRequestContext.GetTextTranslation(text);
        //}

        //protected string GetTextTranslation(Text text, Func<string> alternateTranslationProvider)
        //{
        //    return ApplicationRequestContext.GetTextTranslation(text, alternateTranslationProvider);
        //}

        //protected string GetTextTranslation(Text text, Func<string> alternateTranslationProvider, TextResolveMode resolveModeOverride)
        //{
        //    return ApplicationRequestContext.GetTextTranslation(text, alternateTranslationProvider, resolveModeOverride);
        //}

        protected string GetApplicationText(string text)
        {
            return ApplicationRequestContext.GetApplicationTextTranslation(text);
        }

        //protected string GetTextTranslation(int id)
        //{
        //    return ApplicationRequestContext.TranslationProvider.GetTranslation(id);
        //}

        //protected string GetTextTranslation(int? id)
        //{
        //    return ApplicationRequestContext.TranslationProvider.GetTranslation(id);
        //}

    }

    public interface IViewModel<TModel> : IViewModel
    {
        TModel Model { get; }
    }

    //public abstract class ViewModelBase<TModel> : ViewModelBase, IViewModel<TModel>, IItemSource<TModel>
    //{
    //    protected ViewModelBase(TModel model)
    //    {
    //        if (model == null) throw new ArgumentNullException("model");

    //        Model = model;
    //    }

    //    protected ViewModelBase(TModel model, ApplicationRequestContext applicationRequestContext)
    //        : base(applicationRequestContext)
    //    {
    //        if (model == null) throw new ArgumentNullException("model");

    //        Model = model;
    //    }

    //    public TModel Model { get; private set; }

    //    TModel IItemSource<TModel>.Item => Model;
    //}

    public abstract class ViewModelBase<TModel, TViewModel> : ViewModelBase
    {
        protected ViewModelBase() { }

        protected ViewModelBase(ApplicationRequestContext applicationRequestContext) : base(applicationRequestContext) { }

        protected ViewModelBase(TModel model)
        {
            if (model == null) throw new ArgumentNullException("model");

            Model = model;
        }

        protected ViewModelBase(TModel model, ApplicationRequestContext applicationRequestContext)
            : base(applicationRequestContext)
        {
            if (model == null) throw new ArgumentNullException("model");

            Model = model;
        }


        public TModel Model { get; protected set; }


        public static TViewModel Create(TModel model, ApplicationRequestContext applicationRequestContext)
        {
            return Instance.Create<TModel, ApplicationRequestContext, TViewModel>(model, applicationRequestContext);
        }
    }
}