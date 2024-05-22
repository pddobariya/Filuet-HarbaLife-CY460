using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Conventions
{
    public class NameControllerModelConventionAttribute : Attribute, IControllerModelConvention
    {
        private readonly string _name;

        #region Ctor

        public NameControllerModelConventionAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException();
            _name = name;
        }

        #endregion

        #region Methods

        public void Apply(ControllerModel controller)
        {
            controller.ControllerName = _name;
            foreach (var controllerSelector in controller.Selectors)
            {
                controllerSelector.ActionConstraints.Add(new OrderActionConstraintAttribute(1));
            }
        }

        #endregion
    }

    #region OrderActionConstraintAttribute
    public class OrderActionConstraintAttribute : Attribute, IActionConstraint
    {
        public OrderActionConstraintAttribute(int order)
        {
            Order = order;
        }
        public int Order { get; }

        public bool Accept(ActionConstraintContext context)
        {
            return true;
        }

    }

    #endregion
}
