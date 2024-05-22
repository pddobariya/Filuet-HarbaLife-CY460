using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    public abstract class AuthenticationProxyClientBase<TChannel> : ClientBase<TChannel> where TChannel : class
    {
        #region Fields

        private const string AuthentificationHeader = "Authentification";

        #endregion

        #region Ctor

        protected AuthenticationProxyClientBase(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
        }

        #endregion

        #region Methods

        public void ExecuteInContext(Action execute)
        {
            string authentificationToken = string.Empty;
            using (OperationContextScope scope = new OperationContextScope(this.InnerChannel))
            {
                MessageHeader header = MessageHeader.CreateHeader(AuthentificationHeader, string.Empty, authentificationToken);
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
                execute();
            }
        }

        #endregion
    }
}
