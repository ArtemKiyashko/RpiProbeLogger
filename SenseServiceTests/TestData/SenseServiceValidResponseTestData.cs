using Moq;
using Sense.RTIMU;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SenseServiceTests.TestData
{
    public class SenseServiceValidResponseTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 
                new RTIMUData(It.IsAny<DateTime>(), true, It.IsAny<Vector3>(), true, It.IsAny<Quaternion>(), true, It.IsAny<Vector3>(), true, It.IsAny<Vector3>(), true, It.IsAny<Vector3>()),
                new RTPressureData(true, It.IsAny<float>(), true, It.IsAny<float>()),
                new RTHumidityData(true, It.IsAny<float>(), true, It.IsAny<float>())
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
