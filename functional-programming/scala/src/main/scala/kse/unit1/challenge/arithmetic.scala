package kse.unit1.challenge

import scala.annotation.tailrec

object arithmetic:

  type Number = Long

  val increment: Number => Number =
    value => value + 1

  val decrement: Number => Number =
    value => value - 1

  val isZero: Number => Boolean =
    value => value == 0

  val isNonNegative: Number => Boolean =
    value => value >= 0

  val abs: Number => Number =
    value =>
      if isNonNegative(value) then value
      else -value

  def addition(left: Number, right: Number): Number =
    require(left >= 0, "Left must be non-negative")
    require(right >= 0, "Right must be non-negative")

    @tailrec
    def additionTail(l: Number, r: Number): Number =
      if isZero(r) then l
      else additionTail(increment(l), decrement(r))

    additionTail(left, right)

  def multiplication(left: Number, right: Number): Number =
    require(left >= 0, "Left must be non-negative")
    require(right >= 0, "Right must be non-negative")

    @tailrec
    def multiplicationTail(acc: Number, l: Number, r: Number): Number =
      if isZero(r) then acc
      else multiplicationTail(addition(acc, l), l, decrement(r))

    multiplicationTail(0, left, right)

  def power(base: Number, p: Number): Number =
    require(p >= 0, "Power must be non-negative")
    require(base != 0 || p != 0, "0^0 is undefined")

    @tailrec
    def powerTail(acc: Number, b: Number, p: Number): Number =
      if isZero(p) then acc
      else powerTail(multiplication(acc, b), b, decrement(p))

    powerTail(1, base, p)

end arithmetic
