﻿//-----------------------------------------------------------------------
// <copyright file="TaskTest.cs">
//   MS-PL
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the Microsoft 
//   Public License. A copy of the license can be found in the License.html 
//   file at the root of this distribution. 
//   By using this source code in any fashion, you are agreeing to be bound 
//   by the terms of the Microsoft Public License. You must not remove this 
//   notice, or any other, from this software.
// </license>
//-----------------------------------------------------------------------
namespace VSPackageUnitTest
{
    using StyleCop.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using VSPackageUnitTest.Mocks;

    [TestClass()]
    public class TaskTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        static MockServiceProvider _serviceProvider;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _serviceProvider = new MockServiceProvider();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Utilities.CleanUpTempFiles();

            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.Clear();
        }

        #endregion
    }
}