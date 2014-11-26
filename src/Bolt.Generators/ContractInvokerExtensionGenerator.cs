
namespace Bolt.Generators
{
    public class ContractInvokerExtensionGenerator : ContractGeneratorBase
    {
        public ContractInvokerExtensionGenerator()
        {
            Modifier = "public";
        }

        public ClassDescriptor ContractInvoker { get; set; }

        public string Modifier { get; set; }

        public override void Generate()
        {
            AddUsings(ServerGenerator.BoltServerNamespace, "Owin");

            ClassGenerator generator = CreateClassGenerator(ContractDescriptor);
            generator.Modifier = Modifier + " static";

            generator.GenerateClass((v) =>
            {
                WriteLine("public static IAppBuilder Use{0}(this IAppBuilder app, {1} instance)", ContractDefinition.Name, ContractDefinition.Root.FullName);
                using (WithBlock())
                {
                    WriteLine("return app.Use{0}(new StaticInstanceProvider(instance));", ContractDefinition.Name);
                }

                WriteLine();

                WriteLine("public static IAppBuilder Use{0}(this IAppBuilder app, IInstanceProvider instanceProvider)", ContractDefinition.Name);
                using (WithBlock())
                {
                    WriteLine("var boltExecutor = app.GetBolt();");
                    WriteLine("var invoker = new {0}();", ContractInvoker.FullName);
                    WriteLine("invoker.Descriptor = {0}.Default;", MetadataProvider.GetContractDescriptor(ContractDefinition).FullName);
                    WriteLine("invoker.Init(boltExecutor.Configuration);");
                    WriteLine("invoker.InstanceProvider = instanceProvider;");
                    WriteLine("boltExecutor.Add(invoker);");
                    WriteLine();
                    WriteLine("return app;");
                }
            });

            base.Generate();
        }

        protected override ClassDescriptor CreateDefaultDescriptor()
        {
            return new ClassDescriptor(ContractInvoker.Name + "Extensions", ServerGenerator.BoltServerNamespace);
        }
    }
}