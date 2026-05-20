package kse.unit5.challenge

import kse.unit4.challenge.numerals.Numeral
import scala.annotation.targetName

object set:

  trait NumeralSet:

    infix def forAll(predicate: Numeral => Boolean): Boolean

    infix def exists(predicate: Numeral => Boolean): Boolean

    infix def contains(x: Numeral): Boolean

    infix def include(x: Numeral): NumeralSet

    @targetName("union")
    infix def ∪(that: NumeralSet): NumeralSet

    @targetName("intersection")
    infix def ∩(that: NumeralSet): NumeralSet

  end NumeralSet

  type Empty = Empty.type

  case object Empty extends NumeralSet:

    infix def forAll(predicate: Numeral => Boolean): Boolean = true

    infix def exists(predicate: Numeral => Boolean): Boolean = false

    infix def contains(x: Numeral): Boolean = false

    infix def include(x: Numeral): NumeralSet = NonEmpty(Empty, x, Empty)

    @targetName("union")
    infix def ∪(that: NumeralSet): NumeralSet = that

    @targetName("intersection")
    infix def ∩(that: NumeralSet): NumeralSet = Empty

    override def toString: String = "[*]"

    override def equals(obj: Any): Boolean = obj.isInstanceOf[Empty]

    override def hashCode(): Int = 0

  end Empty

  case class NonEmpty(left: NumeralSet, element: Numeral, right: NumeralSet) extends NumeralSet:

    infix def forAll(predicate: Numeral => Boolean): Boolean =
      predicate(element) && left.forAll(predicate) && right.forAll(predicate)

    infix def exists(predicate: Numeral => Boolean): Boolean =
      predicate(element) || left.exists(predicate) || right.exists(predicate)

    infix def contains(x: Numeral): Boolean =
      if (x < element) left.contains(x)
      else if (x > element) right.contains(x)
      else true

    infix def include(x: Numeral): NumeralSet =
      if (x < element) NonEmpty(left.include(x), element, right)
      else if (x > element) NonEmpty(left, element, right.include(x))
      else this

    @targetName("union")
    infix def ∪(that: NumeralSet): NumeralSet =
      that match
        case Empty                          => this
        case NonEmpty(left, element, right) => this.include(element).∪(left).∪(right)

    @targetName("intersection")
    infix def ∩(that: NumeralSet): NumeralSet =
      if (that.contains(element))
        NonEmpty(left ∩ that, element, right ∩ that)
      else
        (left ∩ that) ∪ (right ∩ that)

    override def toString: String = s"[$left - [$element] - $right]"

    override def equals(obj: Any): Boolean = obj match
      case that: NumeralSet => this.forAll(that.contains) && that.forAll(this.contains)
      case _                => false

    override def hashCode: Int = 31 * (31 * left.hashCode + element.hashCode) + right.hashCode

  end NonEmpty

end set
