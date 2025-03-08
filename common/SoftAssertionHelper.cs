using System;
using System.Collections.Generic;

namespace PlaywrightNUnitDemo
{
    public class SoftAssert
    {
        private List<string> _errorMessages = new List<string>();

        public void AssertTrue(bool condition, string message)
        {
            if (!condition)
            {
                _errorMessages.Add(message);
            }
        }

        public void AssertAreEqual<T>(T expected, T actual, string message)
        {
           try
           {
              Assert.AreEqual(expected, actual, message);
           }
           catch (Exception ex)
           {
              _errorMessages.Add(ex.Message);
           }
        }

        public void AssertAll()
        {
            if (_errorMessages.Count > 0)
            {
                throw new AssertionException(string.Join(Environment.NewLine, _errorMessages));
            }
        }
    }
}
