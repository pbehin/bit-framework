﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Microsoft.CodeAnalysis;
using BitCodeGenerator.Test.Helpers;
using BitCodeGenerator.Implementations;
using BitTools.Core.Model;

namespace BitCodeGenerator.Test.Extensions
{
    [TestClass]
    public class IPropertySymbolExtensionsTests : CodeGeneratorTest
    {
        [TestMethod]
        public void IPropertySymbolExtensionsShouldDeterminateKeyPropertiesAsDesired()
        {
            string dtoCode = @"

public interface IDto {
}

public class SampleDto : IDto {
    public virtual int Id {get;set;}
    public virtual int SampleDtoId {get;set;}
    [KeyAttribute]
    public virtual int SomeKye{get;set;}
    public virtual string NotAKey{get;set;}
}

public class DtoController<TDto>
    where TDto : IDto
{

}

public class SamplesController : DtoController<SampleDto>
{

}

";
            DefaultProjectDtosProvider dtosProvider = new DefaultProjectDtosProvider(new DefaultProjectDtoControllersProvider());

            Dto dto = dtosProvider.GetProjectDtos(CreateProjectFromSourceCodes(dtoCode)).Single();

            Assert.IsFalse(dto.Properties.ElementAt(0).IsKey());
            Assert.IsFalse(dto.Properties.ElementAt(1).IsKey());
            Assert.IsTrue(dto.Properties.ElementAt(2).IsKey());
            Assert.IsFalse(dto.Properties.ElementAt(3).IsKey());
        }

        [TestMethod]
        public void IPropertySymbolExtensionsShouldDeterminateConcurrencyCheckPropertiesAsDesired()
        {
            string dtoCode = @"

public interface IDto {
}

public class SampleDto : IDto {
    [ConcurrencyCheckAttribute]
    public virtual int Concurrency{get;set;}
    public virtual int NonConcurrency{get;set;}

}

public class DtoController<TDto>
    where TDto : IDto
{

}

public class SamplesController : DtoController<SampleDto>
{

}

";
            DefaultProjectDtosProvider dtosProvider = new DefaultProjectDtosProvider(new DefaultProjectDtoControllersProvider());

            Dto dto = dtosProvider.GetProjectDtos(CreateProjectFromSourceCodes(dtoCode)).Single();

            Assert.IsTrue(dto.Properties.ElementAt(0).HasConcurrencyCheck());
            Assert.AreEqual(1, dto.Properties.Count(p => p.HasConcurrencyCheck()));
        }
    }
}
