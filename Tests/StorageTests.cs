using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sales.Storage.Validation;

namespace Tests
{
    [TestClass]
    public class StorageTests
    {
        private Dictionary<string, bool> fileNamesSource = new Dictionary<string, bool>();

        public StorageTests()
        {
            fileNamesSource.Add("", false);            
            fileNamesSource.Add("asdsd", false);
            fileNamesSource.Add("asfde_121212.ll", false);
            fileNamesSource.Add("AlNaz_18121980.json", true);
            fileNamesSource.Add("AlNaz_33121980.json", false);
            fileNamesSource.Add("Iviva_01022010.aa", false);            
        }

        [TestMethod]        
        public void FileNameValidation_Correct()
        {
            foreach (var entry in fileNamesSource)
            {
                var validationResult = FileNameValidator.Validate(entry.Key);
                Assert.AreEqual(entry.Value, validationResult.IsValid);
            }
        }
    }
}
