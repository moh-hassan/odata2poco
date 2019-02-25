using System;
using NUnit.Framework;
using OData2Poco.Exceptions;
using OData2Poco.Extensions;
/*
 * todo
 * file not found
 *
 */
namespace OData2Poco.CommandLine.Test
{
    class OptionManagerTest
    {
        [Test]
        [TestCase("pas")]
        [TestCase("PAS")]
        [TestCase("cam")]
        [TestCase("camel")]
        [TestCase("none")]

        public void NameCase_valid_Test(string nameCase)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = nameCase
            };
            var om = new OptionManager(options);
            //om.Validate();
            //Assert.That(ret, Is.EqualTo(0));
            Assert.That(options.Errors, Is.Empty);
            Console.WriteLine(options.Errors.Dump());
        }
        [Test]
        [TestCase("zz")]
        [TestCase("_")]
        [TestCase("")]
        public void NameCase_invalid_Test(string nameCase)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = nameCase
            };
            var om = new OptionManager(options);
            Assert.That(options.Errors, Is.Not.Empty);
        }

        [Test]
        public void No_paramfile_test()
        {
            var options = new Options
            {
                Url = "http://localhost",
                TokenParams = "client_id=aaaa",
            };
            var om = new OptionManager(options);
            //var connString = options.GetOdataConnectionString();
            //Console.WriteLine(options.ParamFile);
            //Console.WriteLine($"ParamFile {options.ParamFile}");
            var dict = om.EnvironmentVariables;
            Assert.That(dict.Count, Is.EqualTo(0));
           
        }
        [Test]
        public void Read_paramfile_test()
        {
            var options = new Options
            {
                ParamFile = TestSample.Param1,
            };
            var om = new OptionManager(options);
            //var connString = options.GetOdataConnectionString();
            //Console.WriteLine(options.ParamFile);
            //Console.WriteLine($"ParamFile {options.ParamFile}");
            var dict = om.EnvironmentVariables;
            Assert.That(dict.Count, Is.GreaterThan(0));
            Assert.That(dict["url"], Is.EqualTo("http://localhost"));
            //Console.WriteLine(dict.Dump());
        }
        [Test]
        public void Read_postman_json_paramfile_test()
        {
            var options = new Options
            {
                ParamFile = TestSample.PostmanParams,
            };
            var om = new OptionManager(options);
            //var connString = options.GetOdataConnectionString();
            //Console.WriteLine(options.ParamFile);
            //Console.WriteLine($"ParamFile {options.ParamFile}");

            var dict = om.EnvironmentVariables;
            Assert.That(dict.Count, Is.GreaterThan(0));
            Assert.That(dict["url"], Is.EqualTo("https://myorg.crm.dynamics.com"));
            Assert.That(dict["authurl"], Is.EqualTo("https://login.microsoftonline.com/common/oauth2/authorize?resource=https://myorg.crm.dynamics.com"));
            //Console.WriteLine(dict.Dump());
        }
        [Test]
        public void Read_paramfile_with_named_var_from_postman_test()
        {
            var options = new Options
            {
                Url = "{{webapiurl}}",
                TokenEndpoint = "{{authurl}}",
                ParamFile = TestSample.PostmanParams,
            };
            Console.WriteLine(options.Dump());
            var om = new OptionManager(options);
            var expected = "Url= http://localhost";
            //Assert.That(options.TokenParams, Is.EqualTo(expected));
            Console.WriteLine(om.EnvironmentVariables.Dump());
            //Console.WriteLine(options.Dump());
            Console.WriteLine($"Url= {options.Url}");
            Console.WriteLine($"TokenEndPoint= {options.TokenEndpoint}");

            //Url = https://myorg.crm.dynamics.com/api/data/v9.0/
            //TokenEndPoint = https://login.microsoftonline.com/common/oauth2/authorize?resource=https://myorg.crm.dynamics.com

        }
        [Test]
        public void Read_paramfile_with_named_var_options_test()
        {
            var options = new Options
            {
                Url = "{{url}}",
                Password = "{{password}}",
                ParamFile = TestSample.Param1,
            };
            Console.WriteLine(options.Dump());
            var om = new OptionManager(options);
            var expected = "Url= http://localhost";
            //Assert.That(options.TokenParams, Is.EqualTo(expected));
            Console.WriteLine(om.EnvironmentVariables.Dump());
            Console.WriteLine(options.Dump());
            Console.WriteLine($"Password= {options.Password}");
            Console.WriteLine($"url= {options.Url}");

        }
        [Test]
        public void Read_paramfile_not_exist_test4b()
        {
            var options = new Options
            {

                ParamFile = "file_not_exist",
            };
            Assert.That(() => new OptionManager(options),
           Throws.Exception.TypeOf<Odata2PocoException>()
               .With.Property("Message").Contains("Fail to read the parameter file"));
        }
        [Test]
        public void Read_paramfile_var_not_exist_test()
        {
            var options = new Options
            {
                Url = "{{url_1}}",
                ParamFile = TestSample.Param1,
            };
            Assert.That(() => new OptionManager(options),
                Throws.Exception.TypeOf<Odata2PocoException>()
                .With.Property("Message").Contains("Variable not found:url_1"));
        }
        [Test]
        public void Read_paramfile_with_options_include_named_variable_test2()
        {
            var options = new Options
            {
                ParamFile = TestSample.Param1,
                //TokenParams = "resource=http://myorg.com&client_id=?&client_secret=?",
                TokenParams = "resource=http://myorg.com&client_id=?&client_secret=?",
            };

            var om = new OptionManager(options);
            //var expected = "resource=http://myorg.com&client_id=aaaaa-bbbbb-cccccc-ddddd&client_secret=zmu19LGZ6SP8fCGJrCnvqp2/EmlhFr11mVaQsMvL9LA1";
            //Assert.That(options.TokenParams, Is.EqualTo(expected));
            Console.WriteLine($"TokenParams= {options.TokenParams}");
            Console.WriteLine($"TokenParams= {((Options)om).TokenParams}");
        }

        [Test]
        public void Read_paramfile_with_options_include_named_variable_test()
        {
            var options = new Options
            {
                ParamFile = TestSample.Param1,
                TokenParams = "resource=http://myorg.com&client_id={{client_id}}&client_secret={{client_secret}}",
            };

            var om = new OptionManager(options);
            var expected = "resource=http://myorg.com&client_id=aaaaa-bbbbb-cccccc-ddddd&client_secret=zmu19LGZ6SP8fCGJrCnvqp2/EmlhFr11mVaQsMvL9LA1";
            Assert.That(options.TokenParams, Is.EqualTo(expected));
            //Console.WriteLine($"TokenParams= {options.TokenParams}");
        }
        //todo var not exist
        //invalid var {{key}   {key}}
        [Test]
        public void Paramfile_with_placeholder_test()
        {
            var options = new Options
            {
                ParamFile = TestSample.Param1,
                //token_params=client_id=?&client_secret=?&grant_type=?&resource=?
                TokenParams = "{{token_params}}",
            };

            var om = new OptionManager(options);
            var expected =
                @"client_id=aaaaa-bbbbb-cccccc-ddddd&client_secret=zmu19LGZ6SP8fCGJrCnvqp2/EmlhFr11mVaQsMvL9LA1&grant_type=client_credentials&resource=http://myorg.com";
            Assert.That(options.TokenParams, Is.EqualTo(expected));
            //Console.WriteLine($"TokenParams= {options.TokenParams}");
        }
        [Test]
        public void Read_paramfile_with_var_dependon_other_var_test()
        {
            //Arrange
            var options = new Options
            {
                ParamFile = TestSample.Param1,
                Url = "{{url2}}",
            };
            //Act
            OptionManager om = new OptionManager(options);
            //Assert
            Assert.That(options.Url, Is.EqualTo("http://localhost/data"));
        }
        //[Test]
        //[TestCase(@"url=?")]
        //[TestCase(@"token  =  ?")]
        //[TestCase(@"password=  ?")]
        //public void Test1(string text)
        //{

        //    var om = new OptionManager();
        //    var result = om.EvalMacro2(text);
        //    Console.WriteLine($"result= '{result}'");
        //}
    }
}
