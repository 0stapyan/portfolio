package kse.unit4.challenge

import kse.unit4.challenge.generators.given
import kse.unit4.challenge.numerals.*
import org.scalacheck.*
import org.scalacheck.Prop.{forAll, propBoolean, throws}
import org.scalacheck.Test.Parameters

object NumeralsSpecification extends Properties("Numerals"):

  override def overrideParameters(p: Parameters): Parameters =
    p.withMinSuccessfulTests(50).withMaxDiscardRatio(100)

  include(ZeroSpecification)
  include(SuccessorSpecification)
  include(AdditionSpecification)
  include(ComparisonSpecification)
  include(SubtractionSpecification)

end NumeralsSpecification

object ZeroSpecification extends Properties("Zero"):

  property("Zero.isZero must be true") = propBoolean:
    Zero.isZero == true

  property("Zero.predecessor must return Zero") = propBoolean:
    Zero.predecessor == Zero

  property("Zero > any numeral must be false") = forAll: (num: Numeral) =>
    !(Zero > num)

end ZeroSpecification

object SuccessorSpecification extends Properties("Successor"):

  property("Successor(n).isZero must be false") = forAll: (num: Numeral) =>
    !num.successor.isZero

  property("Successor(n).predecessor must be n") = forAll: (num: Numeral) =>
    num.successor.predecessor == num

  property("Successor(a) + b must be Successor(a + b)") = forAll: (a: Numeral, b: Numeral) =>
    (a.successor + b) == (a + b).successor

  property("Successor(a) > a must be true") = forAll: (a: Numeral) =>
    a.successor > a

end SuccessorSpecification

object AdditionSpecification extends Properties("Addition"):

  property("Zero + numeral must be numeral") = forAll: (num: Numeral) =>
    (Zero + num) == num

  property("numeral + Zero must be numeral") = forAll: (num: Numeral) =>
    (num + Zero) == num

  property("Addition must be commutative") = forAll: (a: Numeral, b: Numeral) =>
    (a + b) == (b + a)

  property("Addition must be associative") = forAll: (a: Numeral, b: Numeral, c: Numeral) =>
    ((a + b) + c) == (a + (b + c))

end AdditionSpecification

object ComparisonSpecification extends Properties("Comparison"):

  property("Zero < Successor(n) must be true") = forAll: (num: Numeral) =>
    Zero < num.successor

  property("Successor(n) > Zero must be true") = forAll: (num: Numeral) =>
    num.successor > Zero

  property("a < b implies !(a >= b)") = forAll: (a: Numeral, b: Numeral) =>
    (a < b) == !(a >= b)

  property("a > b implies !(a <= b)") = forAll: (a: Numeral, b: Numeral) =>
    (a > b) == !(a <= b)

  property("a >= b implies a > b  a == b") = forAll: (a: Numeral, b: Numeral) =>
    (a >= b) == (a > b || a == b)

  property("Successor(a) > Successor(b) must be a > b") = forAll: (a: Numeral, b: Numeral) =>
    (a.successor > b.successor) == (a > b)

end ComparisonSpecification

object SubtractionSpecification extends Properties("Subtraction"):

  property("numeral - Zero must be numeral") = forAll: (num: Numeral) =>
    (num - Zero) == num

  property("Zero - numeral must be Zero") = forAll: (num: Numeral) =>
    (Zero - num) == Zero

  property("numeral - numeral must be Zero") = forAll: (num: Numeral) =>
    (num - num) == Zero

  property("Successor(a) - Successor(b) must be a - b") = forAll: (a: Numeral, b: Numeral) =>
    (a.successor - b.successor) == (a - b)

end SubtractionSpecification
