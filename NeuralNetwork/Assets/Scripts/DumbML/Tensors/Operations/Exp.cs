﻿using System.Collections.Generic;
using System;


namespace DumbML {
    public class Exp : Operation {
        Tensor[] error;
        Tensor t;
        public Exp (Operation op) : base(op.shape, op) {
            error = new Tensor[1] {t = result.SameShape() };
        }

        protected override Tensor Compute(Tensor[] operands) {
            for (int i = 0; i < operands[0].Size; i++) {
                result._value[i] = (float)Math.Exp(operands[0]._value[i]);
            }

            return result;
        }
        protected override Tensor[] BackwardsPass(Tensor e) {
            for (int i = 0; i < e.Size; i++) {
                t._value[i] = result._value[i] * e._value[i]; ;
            }
            return error;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Exp(inner[0]._Copy(track));
        }
    }
}