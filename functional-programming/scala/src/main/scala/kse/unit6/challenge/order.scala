package kse.unit6.challenge

import kse.unit4.challenge.numerals.Numeral
import scala.annotation.targetName

object order:

  trait Order[T]:
    def compare(left: T, right: T): Int

  extension [V: Order as ord](elem: V)

    infix def >(that: V): Boolean =
      ord.compare(elem, that) > 0

    infix def <(that: V): Boolean =
      ord.compare(elem, that) < 0

    infix def >=(that: V): Boolean =
      ord.compare(elem, that) >= 0

    infix def =<(that: V): Boolean =
      ord.compare(elem, that) <= 0

  object NumeralOrder:

    given Order[Numeral] with

      def compare(x: Numeral, y: Numeral): Int =
        if x < y then -1
        else if x > y then 1
        else 0
