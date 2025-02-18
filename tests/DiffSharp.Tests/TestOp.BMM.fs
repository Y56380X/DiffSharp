// Copyright (c) 2016-     University of Oxford (Atilim Gunes Baydin <gunes@robots.ox.ac.uk>)
// and other contributors, see LICENSE in root of repository.
//
// BSD 2-Clause License. See LICENSE in root of repository.

namespace Tests

open NUnit.Framework
open DiffSharp


[<TestFixture>]
type TestTensorBMM () =
    [<Test>]
    member _.TestTensorBMM () =
        for combo in Combos.FloatingPoint do
            let t1 = combo.tensor([[[-1.0372e+00,  7.5673e-01,  1.9448e+00,  3.6433e+00, -3.9134e-01],
                                     [-1.7011e+00,  3.0675e+00,  1.8387e+00, -2.3037e-01,  5.0916e-01],
                                     [ 2.1869e+00,  1.5561e+00,  1.2905e+00, -3.5149e-03, -2.0392e+00],
                                     [-1.0669e+00,  3.0033e-01,  1.2472e+00,  7.8584e-01, -5.0704e-01]],

                                    [[-2.8406e-02, -7.2715e-01, -2.6762e-02, -7.6213e-02,  1.3507e+00],
                                     [-1.0652e+00, -8.9129e-01,  1.3157e+00,  1.5385e+00, -7.5446e-02],
                                     [ 1.0338e-01,  9.9040e-02,  8.3478e-01,  2.1243e+00,  1.4483e+00],
                                     [-3.1956e-01,  1.1361e+00,  8.4474e-01, -1.2423e+00, -1.7816e+00]],

                                    [[-4.4167e-01,  4.1456e-01,  9.4991e-01, -1.3340e+00, -2.4315e+00],
                                     [-1.8150e+00,  1.1680e+00, -9.0262e-01, -4.3182e-01,  3.6071e-01],
                                     [-1.3226e-02,  1.0893e+00,  7.8359e-01,  3.6028e-01,  2.2133e-01],
                                     [-1.5645e+00,  2.5328e+00,  1.6512e+00,  1.5900e-01, -1.4043e+00]]])
            let t2 = combo.tensor([[[ 0.3481,  0.9439],
                                     [-0.5359, -1.7125],
                                     [ 1.4898,  0.6685],
                                     [-1.0406,  0.7051],
                                     [-0.2785,  0.6121]],

                                    [[ 0.4999,  0.5361],
                                     [ 3.0867, -0.8065],
                                     [-0.9747, -1.7684],
                                     [-1.5903, -0.6835],
                                     [-0.3202, -2.0839]],

                                    [[ 0.9485,  0.0655],
                                     [-0.5164,  0.4726],
                                     [-0.7083,  0.5276],
                                     [ 0.2138, -0.9645],
                                     [ 0.1784,  0.3654]]])

            let t3 = t1.bmm(t2)
            let t3Correct = combo.tensor([[[-1.5514,  1.3546],
                                             [ 0.6013, -5.4802],
                                             [ 2.4214, -0.9885],
                                             [ 0.6491, -0.4439]],

                                            [[-2.5438, -2.1440],
                                             [-6.9885, -3.0734],
                                             [-4.2981, -5.9709],
                                             [ 5.0698,  1.9803]],

                                            [[-2.0247,  1.0662],
                                             [-1.7133,  0.5051],
                                             [-1.0136,  0.6607],
                                             [-4.1779,  1.2990]]])

            Assert.True(t3Correct.allclose(t3, 0.1))


[<TestFixture>]
type TestDerivativesBMM () =
    [<Test>]
    member _.TestDerivativeBMMTT () =
        for combo in Combos.AllDevicesAndBackendsFloat32 do
            let fwdx = combo.tensor([[[ 0.5113,  1.6059,  0.9279, -1.2155],
                                      [ 1.3993, -0.8222,  1.5630, -0.1304],
                                      [ 1.5626, -0.1561,  2.3937,  0.0247]],
                             
                                     [[ 0.4439, -0.3913, -1.0125, -0.4251],
                                      [ 0.5604,  0.8582,  0.2060, -1.3235],
                                      [ 1.7269, -0.3822,  0.0256,  0.7919]]])
            let fwdx = fwdx.forwardDiff(combo.tensor([[[-1.3354,  0.0961,  1.5172, -0.5800],
                                                          [ 0.6343, -0.0522,  0.5044,  0.3696],
                                                          [-1.1742, -0.0101,  0.6215, -0.8946]],
                                                 
                                                         [[-0.3507, -0.3640, -1.5019,  0.4116],
                                                          [ 0.0315, -0.7933,  0.2258, -0.8157],
                                                          [ 0.9618,  1.4108, -0.5264,  0.2942]]]))
            let fwdy = combo.tensor([[[ 0.7702, -0.3661],
                                      [-1.0287, -1.4380],
                                      [-0.8915, -1.4034],
                                      [-1.8511, -2.8105]],
                             
                                     [[-0.7284, -0.6648],
                                      [ 1.1528,  1.9084],
                                      [ 0.1676, -1.1728],
                                      [ 1.2183, -0.0281]]])
            let fwdy = fwdy.forwardDiff(combo.tensor([[[-0.5935,  0.8928],
                                                          [-0.6523,  1.2540],
                                                          [ 0.9517, -0.3758],
                                                          [ 0.6564, -1.0481]],
                                                 
                                                         [[ 0.1805,  1.3280],
                                                          [-0.8441, -0.8922],
                                                          [ 0.2210, -2.0530],
                                                          [-2.1143,  0.3400]]]))
            let fwdz = dsharp.bmm(fwdx, fwdy)
            let fwdzCorrect = combo.tensor([[[ 0.1647, -0.3824],
                                              [ 0.7714, -1.1569],
                                              [-0.8156, -3.7763]],
                                     
                                             [[-1.4620,  0.1576],
                                              [-0.9967,  1.0609],
                                              [-0.7295, -1.9297]]])
            let fwdzd = fwdz.derivative
            let fwdzdCorrect = combo.tensor([[[-2.6723,  3.2469],
                                              [ 0.5161, -2.1363],
                                              [ 1.6767,  2.3604]],
                                     
                                             [[ 1.1710,  4.1612],
                                              [ 0.3272, -2.6713],
                                              [ 0.1617,  5.5129]]])

            let revx = combo.tensor([[[ 0.5113,  1.6059,  0.9279, -1.2155],
                                      [ 1.3993, -0.8222,  1.5630, -0.1304],
                                      [ 1.5626, -0.1561,  2.3937,  0.0247]],
                             
                                     [[ 0.4439, -0.3913, -1.0125, -0.4251],
                                      [ 0.5604,  0.8582,  0.2060, -1.3235],
                                      [ 1.7269, -0.3822,  0.0256,  0.7919]]]).reverseDiff()
            let revy = combo.tensor([[[ 0.7702, -0.3661],
                                      [-1.0287, -1.4380],
                                      [-0.8915, -1.4034],
                                      [-1.8511, -2.8105]],
                             
                                     [[-0.7284, -0.6648],
                                      [ 1.1528,  1.9084],
                                      [ 0.1676, -1.1728],
                                      [ 1.2183, -0.0281]]]).reverseDiff()
            let revz = dsharp.bmm(revx, revy)
            let revzCorrect = combo.tensor([[[ 0.1647, -0.3824],
                                              [ 0.7714, -1.1569],
                                              [-0.8156, -3.7763]],
                                     
                                             [[-1.4620,  0.1576],
                                              [-0.9967,  1.0609],
                                              [-0.7295, -1.9297]]])
            revz.reverse(combo.tensor([[[-2.8632,  1.7088],
                                         [-1.2570, -0.0673],
                                         [-2.6239, -1.0855]],

                                        [[ 1.0658,  0.5892],
                                         [ 0.2221, -0.6175],
                                         [ 0.5876,  1.1322]]]))            
            let revxd = revx.derivative
            let revxdCorrect = combo.tensor([[[-2.8307,  0.4879,  0.1544,  0.4973],
                                               [-0.9435,  1.3898,  1.2151,  2.5159],
                                               [-1.6235,  4.2601,  3.8627,  7.9078]],
                                      
                                              [[-1.1680,  2.3531, -0.5124,  1.2818],
                                               [ 0.2488, -0.9225,  0.7615,  0.2879],
                                               [-1.1806,  2.8380, -1.2293,  0.6840]]])
            let revyd = revy.derivative
            let revydCorrect = combo.tensor([[[ -7.3228,  -0.9167],
                                               [ -3.1549,   2.9690],
                                               [-10.9021,  -1.1180],
                                               [  3.5795,  -2.0952]],
                                      
                                              [[  1.6123,   1.8706],
                                               [ -0.4510,  -1.1933],
                                               [ -1.0183,  -0.6948],
                                               [ -0.2817,   1.4633]]])

            Assert.True(fwdz.allclose(fwdzCorrect, 0.01))
            Assert.True(fwdzd.allclose(fwdzdCorrect, 0.01))
            Assert.True(revz.allclose(revzCorrect, 0.01))
            Assert.True(revxd.allclose(revxdCorrect, 0.01))
            Assert.True(revyd.allclose(revydCorrect, 0.01))

    [<Test>]
    member _.TestDerivativeBMMTTConst () =
        for combo in Combos.AllDevicesAndBackendsFloat32 do
            let fwdx = combo.tensor([[[ 0.5113,  1.6059,  0.9279, -1.2155],
                                      [ 1.3993, -0.8222,  1.5630, -0.1304],
                                      [ 1.5626, -0.1561,  2.3937,  0.0247]],
                             
                                     [[ 0.4439, -0.3913, -1.0125, -0.4251],
                                      [ 0.5604,  0.8582,  0.2060, -1.3235],
                                      [ 1.7269, -0.3822,  0.0256,  0.7919]]])
            let fwdx = fwdx.forwardDiff(combo.tensor([[[-1.3354,  0.0961,  1.5172, -0.5800],
                                                          [ 0.6343, -0.0522,  0.5044,  0.3696],
                                                          [-1.1742, -0.0101,  0.6215, -0.8946]],
                                                 
                                                         [[-0.3507, -0.3640, -1.5019,  0.4116],
                                                          [ 0.0315, -0.7933,  0.2258, -0.8157],
                                                          [ 0.9618,  1.4108, -0.5264,  0.2942]]]))
            let fwdy = combo.tensor([[[ 0.7702, -0.3661],
                                      [-1.0287, -1.4380],
                                      [-0.8915, -1.4034],
                                      [-1.8511, -2.8105]],
                             
                                     [[-0.7284, -0.6648],
                                      [ 1.1528,  1.9084],
                                      [ 0.1676, -1.1728],
                                      [ 1.2183, -0.0281]]])
            let fwdz = dsharp.bmm(fwdx, fwdy)
            let fwdzCorrect = combo.tensor([[[ 0.1647, -0.3824],
                                              [ 0.7714, -1.1569],
                                              [-0.8156, -3.7763]],
                                     
                                             [[-1.4620,  0.1576],
                                              [-0.9967,  1.0609],
                                              [-0.7295, -1.9297]]])
            let fwdzd = fwdz.derivative
            let fwdzdCorrect = combo.tensor([[[-1.4065, -0.1487],
                                              [-0.5917, -1.9040],
                                              [ 0.2079,  2.0863]],
                                     
                                             [[ 0.0856,  1.2884],
                                              [-1.8934, -1.7768],
                                              [ 1.1959,  2.6619]]])

            let revx = combo.tensor([[[ 0.5113,  1.6059,  0.9279, -1.2155],
                                      [ 1.3993, -0.8222,  1.5630, -0.1304],
                                      [ 1.5626, -0.1561,  2.3937,  0.0247]],
                             
                                     [[ 0.4439, -0.3913, -1.0125, -0.4251],
                                      [ 0.5604,  0.8582,  0.2060, -1.3235],
                                      [ 1.7269, -0.3822,  0.0256,  0.7919]]]).reverseDiff()
            let revy = combo.tensor([[[ 0.7702, -0.3661],
                                      [-1.0287, -1.4380],
                                      [-0.8915, -1.4034],
                                      [-1.8511, -2.8105]],
                             
                                     [[-0.7284, -0.6648],
                                      [ 1.1528,  1.9084],
                                      [ 0.1676, -1.1728],
                                      [ 1.2183, -0.0281]]])
            let revz = dsharp.bmm(revx, revy)
            let revzCorrect = combo.tensor([[[ 0.1647, -0.3824],
                                              [ 0.7714, -1.1569],
                                              [-0.8156, -3.7763]],
                                     
                                             [[-1.4620,  0.1576],
                                              [-0.9967,  1.0609],
                                              [-0.7295, -1.9297]]])
            revz.reverse(combo.tensor([[[-2.8632,  1.7088],
                                         [-1.2570, -0.0673],
                                         [-2.6239, -1.0855]],

                                        [[ 1.0658,  0.5892],
                                         [ 0.2221, -0.6175],
                                         [ 0.5876,  1.1322]]]))
            let revxd = revx.derivative
            let revxdCorrect = combo.tensor([[[-2.8307,  0.4879,  0.1544,  0.4973],
                                              [-0.9435,  1.3898,  1.2151,  2.5159],
                                              [-1.6235,  4.2601,  3.8627,  7.9078]],
                                     
                                             [[-1.1680,  2.3531, -0.5124,  1.2818],
                                              [ 0.2488, -0.9225,  0.7615,  0.2879],
                                              [-1.1806,  2.8380, -1.2293,  0.6840]]])
            let revyd = revy.isNoDiff
            let revydCorrect = true

            Assert.True(fwdz.allclose(fwdzCorrect, 0.01))
            Assert.True(fwdzd.allclose(fwdzdCorrect, 0.01))
            Assert.True(revz.allclose(revzCorrect, 0.01))
            Assert.True(revxd.allclose(revxdCorrect, 0.01))
            Assert.CheckEqual(revydCorrect, revyd)

    [<Test>]
    member _.TestDerivativeBMMTConstT () =
        for combo in Combos.AllDevicesAndBackendsFloat32 do
            let fwdx = combo.tensor([[[ 0.5113,  1.6059,  0.9279, -1.2155],
                                      [ 1.3993, -0.8222,  1.5630, -0.1304],
                                      [ 1.5626, -0.1561,  2.3937,  0.0247]],
                             
                                     [[ 0.4439, -0.3913, -1.0125, -0.4251],
                                      [ 0.5604,  0.8582,  0.2060, -1.3235],
                                      [ 1.7269, -0.3822,  0.0256,  0.7919]]])
            let fwdy = combo.tensor([[[ 0.7702, -0.3661],
                                      [-1.0287, -1.4380],
                                      [-0.8915, -1.4034],
                                      [-1.8511, -2.8105]],
                             
                                     [[-0.7284, -0.6648],
                                      [ 1.1528,  1.9084],
                                      [ 0.1676, -1.1728],
                                      [ 1.2183, -0.0281]]])
            let fwdy = fwdy.forwardDiff(combo.tensor([[[-0.5935,  0.8928],
                                                          [-0.6523,  1.2540],
                                                          [ 0.9517, -0.3758],
                                                          [ 0.6564, -1.0481]],
                                                 
                                                         [[ 0.1805,  1.3280],
                                                          [-0.8441, -0.8922],
                                                          [ 0.2210, -2.0530],
                                                          [-2.1143,  0.3400]]]))
            let fwdz = dsharp.bmm(fwdx, fwdy)
            let fwdzCorrect = combo.tensor([[[ 0.1647, -0.3824],
                                              [ 0.7714, -1.1569],
                                              [-0.8156, -3.7763]],
                                     
                                             [[-1.4620,  0.1576],
                                              [-0.9967,  1.0609],
                                              [-0.7295, -1.9297]]])
            let fwdzd = fwdz.derivative
            let fwdzdCorrect = combo.tensor([[[-1.2658,  3.3956],
                                              [ 1.1078, -0.2323],
                                              [ 1.4688,  0.2741]],
                                     
                                             [[ 1.0855,  2.8728],
                                              [ 2.2206, -0.8945],
                                              [-1.0342,  2.8510]]])

            let revx = combo.tensor([[[ 0.5113,  1.6059,  0.9279, -1.2155],
                                      [ 1.3993, -0.8222,  1.5630, -0.1304],
                                      [ 1.5626, -0.1561,  2.3937,  0.0247]],
                             
                                     [[ 0.4439, -0.3913, -1.0125, -0.4251],
                                      [ 0.5604,  0.8582,  0.2060, -1.3235],
                                      [ 1.7269, -0.3822,  0.0256,  0.7919]]])
            let revy = combo.tensor([[[ 0.7702, -0.3661],
                                      [-1.0287, -1.4380],
                                      [-0.8915, -1.4034],
                                      [-1.8511, -2.8105]],
                             
                                     [[-0.7284, -0.6648],
                                      [ 1.1528,  1.9084],
                                      [ 0.1676, -1.1728],
                                      [ 1.2183, -0.0281]]]).reverseDiff()
            let revz = dsharp.bmm(revx, revy)
            let revzCorrect = combo.tensor([[[ 0.1647, -0.3824],
                                              [ 0.7714, -1.1569],
                                              [-0.8156, -3.7763]],
                                     
                                             [[-1.4620,  0.1576],
                                              [-0.9967,  1.0609],
                                              [-0.7295, -1.9297]]])
            revz.reverse(combo.tensor([[[-2.8632,  1.7088],
                                         [-1.2570, -0.0673],
                                         [-2.6239, -1.0855]],

                                        [[ 1.0658,  0.5892],
                                         [ 0.2221, -0.6175],
                                         [ 0.5876,  1.1322]]]))
            let revxd = revx.isNoDiff
            let revxdCorrect = true
            let revyd = revy.derivative
            let revydCorrect = combo.tensor([[[ -7.3228,  -0.9167],
                                              [ -3.1549,   2.9690],
                                              [-10.9021,  -1.1180],
                                              [  3.5795,  -2.0952]],
                                     
                                             [[  1.6123,   1.8706],
                                              [ -0.4510,  -1.1933],
                                              [ -1.0183,  -0.6948],
                                              [ -0.2817,   1.4633]]])

            Assert.True(fwdz.allclose(fwdzCorrect, 0.01))
            Assert.True(fwdzd.allclose(fwdzdCorrect, 0.01))
            Assert.True(revz.allclose(revzCorrect, 0.01))
            Assert.CheckEqual(revxdCorrect, revxd)
            Assert.True(revyd.allclose(revydCorrect, 0.01))
