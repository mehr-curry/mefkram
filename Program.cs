using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO.Compression;

namespace mefkram
{
    class Program
    {
        static void Main(string[] args)
        {
            var registration = new ConventionBuilder();
            registration.ForTypesDerivedFrom<IInterface1>().ImportProperties(pi => pi.Name == nameof(IInterface1.Interfaces))    
                                                           .ImportProperties(pi => pi.Name == nameof(IInterface1.Interface2), (pi, icb) => icb.AddMetadataConstraint("Active", nameof(Object2B)))
                                                           .Export<IInterface1>();
            // Das hier funktioniert leider nicht für Interfaces. Letztendlich wird die PropertyInfo des Interfaces gegen die PropertyInfo der implementierenden Klasse geprüft. Und wird nur ein Standardobjektvergleich durchgeführt.
            //registration.ForType<IInterface1>().ImportProperty(i => i.Interface2).Export<IInterface1>();
            //registration.ForTypesDerivedFrom<IInterface1>().ImportProperties(i => i.PropertyType == typeof(IInterface2)).Export<IInterface1>();
            registration.ForTypesDerivedFrom<IInterface2>().Export<IInterface2>();
            registration.ForTypesMatching(t => t.Name == nameof(Object2B)).Export(builder => builder.AddMetadata("Active", nameof(Object2B)));
//            registration.ForType<Object1>().ImportProperties(i => i.PropertyType == typeof(IInterface2)).Export<Object1>();

            var configuration = new ContainerConfiguration().WithAssembly(typeof(Program).Assembly, registration);
            var container = configuration.CreateContainer();
            
            var resolvedType0 = container.GetExport<IInterface1>();
//            var resolvedType1 = container.GetExport<Object1>();
//            var resolvedType2 = container.GetExport<Object2>();
//            var resolvedType3 = container.GetExport<ExternObject>();
            
//            IInterface1 externObject = new Object1();
//            
//            container.SatisfyImports(externObject, registration);

            Console.Read();
        }
    }

    public interface IInterface1
    {
        IInterface2 Interface2 { get; set; }
        IEnumerable<IInterface2> Interfaces { get; set; }
        string SomeString { get; set; }
    }
    
    public class Object1 : IInterface1
    {
        public IInterface2 Interface2 { get; set; }
        public IEnumerable<IInterface2> Interfaces { get; set; }
        public string SomeString { get; set; }

        public Object1(){}
    }

    public interface IInterface2
    {
        
    }
    
    public class Object2A : IInterface2
    {
        public Object2A(){}
    }
    public class Object2B : IInterface2
    {
        public Object2B(){}
    }
}