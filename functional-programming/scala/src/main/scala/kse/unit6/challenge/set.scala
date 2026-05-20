package kse.unit6.challenge

import kse.unit6.challenge.order.{Order, *, given}
import scala.annotation.targetName

object set:

  trait Set[+A]:

    infix def forAll(predicate: A => Boolean): Boolean

    infix def exists(predicate: A => Boolean): Boolean

    infix def contains[B >: A: Order](x: B): Boolean

    infix def include[B >: A: Order](x: B): Set[B]

    // Optional from the Unit 5. If you haven't implement it in Unit 5 then skip it
    infix def remove[B >: A: Order](x: B): Set[B]

    @targetName("union")
    infix def ∪[B >: A: Order](that: Set[B]): Set[B]

    @targetName("intersection")
    infix def ∩[B >: A: Order](that: Set[B]): Set[B]

    // Optional from the Unit 5. If you haven't implement it in Unit 5 then skip it
    @targetName("difference")
    infix def \[B >: A: Order](that: Set[B]): Set[B]

    // Optional from the Unit 5. If you haven't implement it in Unit 5 then skip it
    @targetName("symmetric difference")
    infix def ∆[B >: A: Order](that: Set[B]): Set[B] = (this \ that) ∪ (that \ this)

  end Set

  type Empty = Empty.type

  // TODO: Remind about type system
  case object Empty extends Set[Nothing]:

    infix def forAll(predicate: Nothing => Boolean): Boolean = true

    infix def exists(predicate: Nothing => Boolean): Boolean = false

    infix def contains[B: Order](x: B): Boolean = false

    infix def include[B: Order](x: B): Set[B] = NonEmpty(Empty, x, Empty)

    // Optional from the Unit 5. If you haven't implement it in Unit 5 then skip it
    infix def remove[B: Order](x: B): Set[B] = this

    @targetName("union")
    infix def ∪[B: Order](that: Set[B]): Set[B] = that

    @targetName("intersection")
    infix def ∩[B: Order](that: Set[B]): Set[B] = this

    // Optional from the Unit 5. If you haven't implement it in Unit 5 then skip it
    @targetName("difference")
    infix def \[B: Order](that: Set[B]): Set[B] = this

    override def toString: String = "[*]"

    override def equals(obj: Any): Boolean =
      obj match
        case _: Empty => true
        case _        => false

    override def hashCode: Int = 0
  end Empty

  case class NonEmpty[A](left: Set[A], element: A, right: Set[A]) extends Set[A]:

    infix def forAll(predicate: A => Boolean): Boolean =
      predicate(element) && left.forAll(predicate) && right.forAll(predicate)

    infix def exists(predicate: A => Boolean): Boolean =
      predicate(element) || left.exists(predicate) || right.exists(predicate)

    infix def contains[B >: A: Order](x: B): Boolean =
      if x < element then left.contains(x)
      else if x > element then right.contains(x)
      else true

    infix def include[B >: A: Order](x: B): Set[B] =
      if x < element then NonEmpty(left.include(x), element, right)
      else if x > element then NonEmpty(left, element, right.include(x))
      else this

    // Optional from the Unit 5. If you haven't implement it in Unit 5 then skip it
    infix def remove[B >: A: Order](x: B): Set[B] =
      if x < element then NonEmpty(left.remove(x), element, right)
      else if x > element then NonEmpty(left, element, right.remove(x))
      else left ∪ right

    @targetName("union")
    infix def ∪[B >: A: Order](that: Set[B]): Set[B] =
      val ord = summon[Order[B]]
      (left.∪(right)(using ord)).∪(that.include[B](element)(using ord))(using ord)

    @targetName("intersection")
    infix def ∩[B >: A: Order](that: Set[B]): Set[B] =
      val ord      = summon[Order[B]]
      val newLeft  = left.∩(that)(using ord)
      val newRight = right.∩(that)(using ord)
      if that.contains[B](element)(using ord) then NonEmpty(newLeft, element, newRight)
      else newLeft.∪(newRight)(using ord)

    // Optional from the Unit 5. If you haven't implement it in Unit 5 then skip it
    @targetName("difference")
    infix def \[B >: A: Order](that: Set[B]): Set[B] =
      val ord      = summon[Order[B]]
      val newLeft  = left.\(that)(using ord)
      val newRight = right.\(that)(using ord)
      if that.contains[B](element)(using ord) then newLeft.∪(newRight)(using ord)
      else NonEmpty(newLeft, element, newRight)

    override def toString: String = s"[$left - [$element] - $right]"

    def isSubsetOf(that: Set[?]): Boolean =
      this.forAll(element => that.exists(_ == element))

    override def equals(obj: Any): Boolean =
      obj match
        case that: NonEmpty[_] => that.isSubsetOf(this) && this.isSubsetOf(that)
        case _                 => false

    override def hashCode: Int =
      element.hashCode + left.hashCode + right.hashCode

  end NonEmpty
end set
