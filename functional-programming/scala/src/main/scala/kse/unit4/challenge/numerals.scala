package kse.unit4.challenge

import scala.annotation.targetName

object numerals:

  trait Numeral:

    def isZero: Boolean

    def predecessor: Numeral

    def successor: Numeral = Successor(this)

    @targetName("greater than")
    infix def >(that: Numeral): Boolean

    @targetName("greater or equal to")
    infix def >=(that: Numeral): Boolean = this > that || this == that

    @targetName("less than")
    infix def <(that: Numeral): Boolean = !(this >= that)

    @targetName("less or equal to")
    infix def <=(that: Numeral): Boolean = !(this > that)

    @targetName("addition")
    infix def +(that: Numeral): Numeral

    // Optional
    @targetName("subtraction")
    infix def -(that: Numeral): Numeral

    def toInt: Int

    override def toString: String = s"Nat($predecessor)"

  type Zero = Zero.type

  object Zero extends Numeral:

    def isZero: Boolean = true

    def predecessor: Numeral = Zero

    @targetName("greater than")
    infix def >(that: Numeral): Boolean = false

    @targetName("addition")
    infix def +(that: Numeral): Numeral = that

    // Optional
    @targetName("subtraction")
    override infix def -(that: Numeral): Numeral = Zero

    def toInt: Int = 0

    override def toString: String = "Zero"

    override def equals(obj: Any): Boolean = obj.isInstanceOf[Zero]

    override def hashCode(): Int = 0

  object Successor:
    def unapply(successor: Successor): Option[Numeral] = Option(successor.predecessor)

  class Successor(n: Numeral) extends Numeral:

    def isZero: Boolean = false

    def predecessor: Numeral = n

    @targetName("greater than")
    infix def >(that: Numeral): Boolean = that match
      case Zero            => true
      case Successor(pred) => predecessor > pred

    @targetName("addition")
    infix def +(that: Numeral): Numeral = Successor(predecessor + that)

    // Optional
    @targetName("subtraction")
    override infix def -(that: Numeral): Numeral = that match
      case Zero            => this
      case Successor(pred) => this.predecessor - pred

    def toInt: Int = 1 + n.toInt

    override def toString: String = s"Successor(${n.toString})"

    override def equals(obj: Any): Boolean = obj match
      case that: Successor => that.predecessor == this.predecessor
      case _               => false

    override def hashCode(): Int = 31 * predecessor.hashCode()
