
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace MultivendorWebViewer.Components
{
    //public class IJsonSerializable
    //{
    //    string Serialize();
    //}

    public class JsonNetModelBinder : IModelBinder
    {

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            if (valueResult != null && valueResult.RawValue != null)
            {
                try
                {
                    string[] strValues = valueResult.RawValue as string[];
                    object value = JsonConvert.DeserializeObject(strValues[0].ToString(), bindingContext.ModelType);
                    var modelBase = value as ViewModels.ViewModelBase;
                    if (modelBase != null && modelBase.ApplicationRequestContext == null)
                    {
                        var applicationRequestContext = controllerContext.GetApplicationRequestContext();
                        modelBase.ApplicationRequestContext = applicationRequestContext;
                    }
                    return value;
                }
                catch (Exception e)
                {
                    modelState.Errors.Add(e);
                }
            }

            return null;
        }

    }

    public class DecimalModelBinder : IModelBinder
    {

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;

            try
            {
                //Check if this is a nullable decimal and a null or empty string has been passed
                var isNullableAndNull = bindingContext.ModelMetadata.IsNullableValueType && string.IsNullOrEmpty(valueResult.AttemptedValue);

                //If not nullable and null then we should try and parse the decimal
                if (!isNullableAndNull)
                {
                    var applicationRequestContext = controllerContext.GetApplicationRequestContext();
                    var state = applicationRequestContext.State;
                    var culture = (state != null ? state.UICulture : null) ?? CultureInfo.CurrentUICulture;
                    decimal decimalValue;
                    if (decimal.TryParse(valueResult.AttemptedValue, NumberStyles.Any, culture, out decimalValue) == true)
                    {
                        actualValue = decimalValue;
                    }
                    else
                    {
                        modelState.Errors.Add(string.Format("Could not convert {0} to decimal", valueResult.AttemptedValue));
                    }                 
                }
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return actualValue;
        }

    }
}