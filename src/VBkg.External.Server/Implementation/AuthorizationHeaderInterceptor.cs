using Grpc.Core;
using Grpc.Core.Interceptors;

namespace VBkg.External.Server.Implementation
{
    internal class AuthorizationHeaderInterceptor : Interceptor
    {
        private readonly string _token;

        public AuthorizationHeaderInterceptor(string token)
        {
            _token = token;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            AddAuthorizationHeader(ref context);

            return base.AsyncUnaryCall(request, context, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
            BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            AddAuthorizationHeader(ref context);

            return base.BlockingUnaryCall(request, context, continuation);
        }

        private void AddAuthorizationHeader<TRequest, TResponse>(ref ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var metadata = new Metadata
            {
                { "Authorization", _token }
            };

            var callOptions = context.Options.WithHeaders(metadata);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOptions);
        }
    }
}