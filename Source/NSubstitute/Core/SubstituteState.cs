﻿using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core
{
    public class SubstituteState : ISubstituteState
    {
        public ISubstitutionContext SubstitutionContext { get; private set; }
        public ICallCollection CallCollection { get; }
        public IReceivedCalls ReceivedCalls { get; private set; }
        public IPendingSpecification PendingSpecification { get; }
        public ICallResults CallResults { get; private set; }
        public ICallSpecificationFactory CallSpecificationFactory { get; private set; }
        public ICallActions CallActions { get; private set; }
        public ICallBaseExclusions CallBaseExclusions { get; private set; }
        public SubstituteConfig SubstituteConfig { get; set; }
        public SequenceNumberGenerator SequenceNumberGenerator { get; private set; }
        public IConfigureCall ConfigureCall { get; private set; }
        public IEventHandlerRegistry EventHandlerRegistry { get; private set; }
        public IAutoValueProvider[] AutoValueProviders { get; private set; }
        public ICallResults AutoValuesCallResults { get; }
        public IResultsForType ResultsForType { get; private set; }
        public ICustomHandlers CustomHandlers { get; }

        public SubstituteState(ISubstitutionContext substitutionContext, SubstituteConfig option)
        {
            SubstitutionContext = substitutionContext;
            SubstituteConfig = option;
            SequenceNumberGenerator = substitutionContext.SequenceNumberGenerator;
            var substituteFactory = substitutionContext.SubstituteFactory;
            var callInfoFactory = new CallInfoFactory();

            var callCollection = new CallCollection();
            CallCollection = callCollection;
            ReceivedCalls = callCollection;

            PendingSpecification = new PendingSpecification(substitutionContext);
            CallResults = new CallResults(callInfoFactory);
            AutoValuesCallResults = new CallResults(callInfoFactory);
            CallSpecificationFactory = CallSpecificationFactoryFactoryYesThatsRight.CreateCallSpecFactory();
            CallActions = new CallActions(callInfoFactory);
            CallBaseExclusions = new CallBaseExclusions();
            ResultsForType = new ResultsForType(callInfoFactory);
            CustomHandlers = new CustomHandlers(this);
            var getCallSpec = new GetCallSpec(callCollection, PendingSpecification, CallSpecificationFactory, CallActions);
            ConfigureCall = new ConfigureCall(CallResults, CallActions, getCallSpec);
            EventHandlerRegistry = new EventHandlerRegistry();

            AutoValueProviders = new IAutoValueProvider[] { 
#if NET45 || NETSTANDARD1_5
                new AutoObservableProvider(() => AutoValueProviders),
                new AutoQueryableProvider(),
#endif
                new AutoSubstituteProvider(substituteFactory),
                new AutoStringProvider(),
                new AutoArrayProvider(),
#if (NET4 || NET45 || NETSTANDARD1_5)
                new AutoTaskProvider(() => AutoValueProviders),
#endif
            };
        }
    }
}
