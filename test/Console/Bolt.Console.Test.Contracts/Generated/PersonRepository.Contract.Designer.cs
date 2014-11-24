//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.

//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Bolt.Console.Test.Contracts;
using Bolt.Console.Test.Contracts.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Bolt.Console.Test.Contracts.Parameters
{
    [DataContract]
    public partial class UpdatePersonParameters
    {
        [DataMember(Order = 1)]
        public Person Person { get; set; }
    }

    [DataContract]
    public partial class UpdatePersonThatThrowsInvalidOperationExceptionParameters
    {
        [DataMember(Order = 1)]
        public Person Person { get; set; }
    }

    [DataContract]
    public partial class DoLongRunningOperationAsyncParameters
    {
        [DataMember(Order = 1)]
        public Person Person { get; set; }
    }

    [DataContract]
    public partial class DoLongRunningOperation2AsyncParameters
    {
    }

    [DataContract]
    public partial class DoNothingWithComplexParameterAsAsyncParameters
    {
        [DataMember(Order = 1)]
        public List<Person> Person { get; set; }
    }

    [DataContract]
    public partial class DoNothingWithComplexParameterParameters
    {
        [DataMember(Order = 1)]
        public List<Person> Person { get; set; }
    }

    [DataContract]
    public partial class GetSimpleTypeParameters
    {
        [DataMember(Order = 1)]
        public int Arg { get; set; }
    }

    [DataContract]
    public partial class GetSimpleTypeAsAsyncParameters
    {
        [DataMember(Order = 1)]
        public int Arg { get; set; }
    }

    [DataContract]
    public partial class GetSinglePersonParameters
    {
        [DataMember(Order = 1)]
        public Person Person { get; set; }
    }

    [DataContract]
    public partial class GetSinglePersonAsAsyncParameters
    {
        [DataMember(Order = 1)]
        public Person Person { get; set; }
    }

    [DataContract]
    public partial class GetManyPersonsParameters
    {
        [DataMember(Order = 1)]
        public Person Person { get; set; }
    }

    [DataContract]
    public partial class GetManyPersonsAsAsyncParameters
    {
        [DataMember(Order = 1)]
        public Person Person { get; set; }
    }

}

namespace Bolt.Console.Test.Contracts
{
    public partial class PersonRepositoryDescriptor : Bolt.ContractDescriptor
    {
        public PersonRepositoryDescriptor() : base(typeof(Bolt.Console.Test.Contracts.IPersonRepository), "PersonRepository")
        {
            UpdatePerson = Add("UpdatePerson", typeof(Bolt.Console.Test.Contracts.Parameters.UpdatePersonParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("UpdatePerson"));
            UpdatePersonThatThrowsInvalidOperationException = Add("UpdatePersonThatThrowsInvalidOperationException", typeof(Bolt.Console.Test.Contracts.Parameters.UpdatePersonThatThrowsInvalidOperationExceptionParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("UpdatePersonThatThrowsInvalidOperationException"));
            DoLongRunningOperationAsync = Add("DoLongRunningOperationAsync", typeof(Bolt.Console.Test.Contracts.Parameters.DoLongRunningOperationAsyncParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("DoLongRunningOperationAsync"));
            DoLongRunningOperation2Async = Add("DoLongRunningOperation2Async", typeof(Bolt.Console.Test.Contracts.Parameters.DoLongRunningOperation2AsyncParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("DoLongRunningOperation2Async"));
            DoNothingAsAsync = Add("DoNothingAsAsync", typeof(Bolt.Empty), typeof(IPersonRepository).GetTypeInfo().GetMethod("DoNothingAsAsync"));
            DoNothing = Add("DoNothing", typeof(Bolt.Empty), typeof(IPersonRepository).GetTypeInfo().GetMethod("DoNothing"));
            DoNothingWithComplexParameterAsAsync = Add("DoNothingWithComplexParameterAsAsync", typeof(Bolt.Console.Test.Contracts.Parameters.DoNothingWithComplexParameterAsAsyncParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("DoNothingWithComplexParameterAsAsync"));
            DoNothingWithComplexParameter = Add("DoNothingWithComplexParameter", typeof(Bolt.Console.Test.Contracts.Parameters.DoNothingWithComplexParameterParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("DoNothingWithComplexParameter"));
            GetSimpleType = Add("GetSimpleType", typeof(Bolt.Console.Test.Contracts.Parameters.GetSimpleTypeParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("GetSimpleType"));
            GetSimpleTypeAsAsync = Add("GetSimpleTypeAsAsync", typeof(Bolt.Console.Test.Contracts.Parameters.GetSimpleTypeAsAsyncParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("GetSimpleTypeAsAsync"));
            GetSinglePerson = Add("GetSinglePerson", typeof(Bolt.Console.Test.Contracts.Parameters.GetSinglePersonParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("GetSinglePerson"));
            GetSinglePersonAsAsync = Add("GetSinglePersonAsAsync", typeof(Bolt.Console.Test.Contracts.Parameters.GetSinglePersonAsAsyncParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("GetSinglePersonAsAsync"));
            GetManyPersons = Add("GetManyPersons", typeof(Bolt.Console.Test.Contracts.Parameters.GetManyPersonsParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("GetManyPersons"));
            GetManyPersonsAsAsync = Add("GetManyPersonsAsAsync", typeof(Bolt.Console.Test.Contracts.Parameters.GetManyPersonsAsAsyncParameters), typeof(IPersonRepository).GetTypeInfo().GetMethod("GetManyPersonsAsAsync"));
            InnerOperation = Add("InnerOperation", typeof(Bolt.Empty), typeof(IPersonRepositoryInner).GetTypeInfo().GetMethod("InnerOperation"));
            InnerOperationExAsync = Add("InnerOperationExAsync", typeof(Bolt.Empty), typeof(IPersonRepositoryInner).GetTypeInfo().GetMethod("InnerOperationExAsync"));
            InnerOperation2 = Add("InnerOperation2", typeof(Bolt.Empty), typeof(IPersonRepositoryInner2).GetTypeInfo().GetMethod("InnerOperation2"));
            InnerOperationExAsync2 = Add("InnerOperationExAsync2", typeof(Bolt.Empty), typeof(IPersonRepositoryInner2).GetTypeInfo().GetMethod("InnerOperationExAsync2"));
        }

        public static readonly PersonRepositoryDescriptor Default = new PersonRepositoryDescriptor();

        public virtual Bolt.ActionDescriptor UpdatePerson { get; set; }

        public virtual Bolt.ActionDescriptor UpdatePersonThatThrowsInvalidOperationException { get; set; }

        public virtual Bolt.ActionDescriptor DoLongRunningOperationAsync { get; set; }

        public virtual Bolt.ActionDescriptor DoLongRunningOperation2Async { get; set; }

        public virtual Bolt.ActionDescriptor DoNothingAsAsync { get; set; }

        public virtual Bolt.ActionDescriptor DoNothing { get; set; }

        public virtual Bolt.ActionDescriptor DoNothingWithComplexParameterAsAsync { get; set; }

        public virtual Bolt.ActionDescriptor DoNothingWithComplexParameter { get; set; }

        public virtual Bolt.ActionDescriptor GetSimpleType { get; set; }

        public virtual Bolt.ActionDescriptor GetSimpleTypeAsAsync { get; set; }

        public virtual Bolt.ActionDescriptor GetSinglePerson { get; set; }

        public virtual Bolt.ActionDescriptor GetSinglePersonAsAsync { get; set; }

        public virtual Bolt.ActionDescriptor GetManyPersons { get; set; }

        public virtual Bolt.ActionDescriptor GetManyPersonsAsAsync { get; set; }

        public virtual Bolt.ActionDescriptor InnerOperation { get; set; }

        public virtual Bolt.ActionDescriptor InnerOperationExAsync { get; set; }

        public virtual Bolt.ActionDescriptor InnerOperation2 { get; set; }

        public virtual Bolt.ActionDescriptor InnerOperationExAsync2 { get; set; }
    }
}
