using System;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;
namespace DumbML {

    public class WrongShapeException : Exception {
        public WrongShapeException(string message) : base(message) {
        }
    }

}