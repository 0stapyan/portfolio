package kse.unit6.challenge

import kse.unit4.challenge.generators.given
import kse.unit4.challenge.numerals.Numeral
import kse.unit6.algebra.{*, given}
import kse.unit6.challenge.generators.given
import kse.unit6.challenge.order.{*, given}
import kse.unit6.challenge.order.NumeralOrder.given
import kse.unit6.challenge.set.*
import org.scalacheck.Prop.{forAll, propBoolean}
import org.scalacheck.Properties

object GeneralSetSpecification extends Properties("Set"):

  include(EmptySpecification)
  include(NonEmptySpecification)
  include(SetSpecification)

end GeneralSetSpecification

object EmptySpecification extends Properties("Empty set laws"):

  property("Universal quantifier applied to the empty set should return true for any predicate") = Empty.forAll(_ => false) && Empty.forAll(_ => true)

  property("Existential quantifier applied to the empty set should return false for any predicate") = !Empty.exists(_ => true) && !Empty.exists(_ => false)

  property("Empty set should not contain any element") = forAll: (numeral: Numeral) =>
    !Empty.contains(numeral)

end EmptySpecification

object NonEmptySpecification extends Properties("Non-empty set laws"):

  property("Universal quantifier applied to an non-empty set should return true for a tautology") = forAll: (nonEmpty: NonEmpty[Numeral]) =>
    nonEmpty.forAll(_ => true)

  property("Universal quantifier applied to an non-empty set should return false for a contradiction") = forAll: (nonEmpty: NonEmpty[Numeral]) =>
    !nonEmpty.forAll(_ => false)

  property("Existential quantifier applied to an non-empty set should return true for a tautology") = forAll: (nonEmpty: NonEmpty[Numeral]) =>
    nonEmpty.exists(_ => true)

  property("Existential quantifier applied to an non-empty set should return false for a contradiction") = forAll: (nonEmpty: NonEmpty[Numeral]) =>
    !nonEmpty.exists(_ => false)

  property("Single-element set should contain an element") = forAll: (numeral: Numeral) =>
    NonEmpty(Empty, numeral, Empty).contains(numeral)

end NonEmptySpecification

object SetSpecification extends Properties("Set laws"):

  property("Universal quantifier applied to a set should return true for a tautology") = forAll: (set: Set[Numeral]) =>
    set.forAll(_ => true)

  property("Any set should contain its elements") = forAll: (set: Set[Numeral], numeral: Numeral) =>
    set.forAll(e => set.contains(e))

  property("If a set does not contain a given element then there is no element in the set equal to the given element") = forAll:
    (set: Set[Numeral], numeral: Numeral) => !set.contains(numeral) ==> (!set.exists(_ == numeral))

  property("If a given element is added to a set then there is an element in the set equals to the give element") = forAll:
    (set: Set[Numeral], numeral: Numeral) => set.include(numeral).exists(_ == numeral)

  property("If a given element is added to a set then set should include the given element") = forAll: (set: Set[Numeral], numeral: Numeral) =>
    set.include(numeral).contains(numeral)

  property("Inclusion should be idempotent") = forAll: (set: Set[Numeral], numeral: Numeral) =>
    set.include(numeral) == set.include(numeral).include(numeral)

  // Optional
  // Uncomment if needed
  property("If a given element is removed from a set then all elements of the set should not be equals to the given element") = forAll:
    (set: Set[Numeral], numeral: Numeral) => set.remove(numeral).forAll(_ != numeral)

  // Optional
  // Uncomment if needed
  property("If a given element is removed from a set then the set should not contain the given element") = forAll: (set: Set[Numeral], numeral: Numeral) =>
    !set.remove(numeral).contains(numeral)

  // Optional
  // Uncomment if needed
  property("If a given new element is added to a set and then removed from the set then the set should not be changed") = forAll:
    (set: Set[Numeral], numeral: Numeral) =>
      if (!set.contains(numeral)) then (set.include(numeral)).remove(numeral) == set
      else true

  property("Removal should be idempotent") = forAll: (set: Set[Numeral], numeral: Numeral) =>
    set.remove(numeral).remove(numeral) == set.remove(numeral)

  property("A union of two given sets should contain the elements from both sets") = forAll: (left: Set[Numeral], right: Set[Numeral]) =>
    (left ∪ right).forAll(e => left.contains(e) || right.contains(e))

  property("An intersections of two given sets should contain the same elements from both sets") = forAll: (left: Set[Numeral], right: Set[Numeral]) =>
    val inter = left ∩ right
    val cond1 = inter.forAll(e => left.contains(e) && right.contains(e))
    val cond2 = left.forAll(e => if (right.contains(e)) inter.contains(e) else true)
    cond1 && cond2

  // Optional.
  // Uncomment if needed
  property("A difference of two given sets should contain elements from a base set but should not contain elements from an exclusion set") = forAll:
    (left: Set[Numeral], right: Set[Numeral]) =>
      val diff  = left \ right
      val cond1 = diff.forAll(e => left.contains(e) && !right.contains(e))
      val cond2 = left.forAll(e => if (!right.contains(e)) diff.contains(e) else true)
      cond1 && cond2

  // Optional.
  // Uncomment if needed
  property("A disjunctive union of two given sets should include elements from a union of the sets but exclude elements from an intersection of the sets") =
    forAll: (left: Set[Numeral], right: Set[Numeral]) =>
      val diff = left \ right
      diff.forAll(e => left.contains(e) && !right.contains(e)) &&
      left.forAll(e => if (!right.contains(e)) diff.contains(e) else true)

  // TODO: The next part should be implemented by students
  property("Union left unit") = forAll: (set: Set[Numeral]) =>
    (Empty ∪ set) == set

  property("Union right unit") = forAll: (set: Set[Numeral]) =>
    (set ∪ Empty) == set

  property("Intersection left zero") = forAll: (set: Set[Numeral]) =>
    (Empty ∩ set) == Empty

  property("Intersection right zero") = forAll: (set: Set[Numeral]) =>
    (set ∩ Empty) == Empty

  // Optional
  // Uncomment if needed
  property("Difference left zero") = forAll: (set: Set[Numeral]) =>
    (Empty \ set) == Empty

  // Optional
  // Uncomment if needed
  property("Difference right unit") = forAll: (set: Set[Numeral]) =>
    (set \ Empty) == set

  // Optional
  // Uncomment if needed
  property("Symmetric difference left unit") = forAll: (set: Set[Numeral]) =>
    (Empty ∆ set) == set

  // Optional
  // Uncomment if needed
  property("Symmetric difference right unit") = forAll: (set: Set[Numeral]) =>
    (set ∆ Empty) == set

  property("Union should be idempotent") = forAll: (set: Set[Numeral]) =>
    (set ∪ set) == set

  property("Intersection should be idempotent") = forAll: (set: Set[Numeral]) =>
    (set ∩ set) == set

  // Optional
  // Uncomment if needed
  property("Difference should be self-inverted") = forAll: (set: Set[Numeral]) =>
    (set \ set) == Empty

  property("Union should be commutative") = forAll: (left: Set[Numeral], right: Set[Numeral]) =>
    (left ∪ right) == (right ∪ left)

  property("Intersection should be commutative") = forAll: (left: Set[Numeral], right: Set[Numeral]) =>
    (left ∩ right) == (right ∩ left)

  property("Union should be associative") = forAll: (a: Set[Numeral], b: Set[Numeral], c: Set[Numeral]) =>
    ((a ∪ b) ∪ c) == (a ∪ (b ∪ c))

  property("Intersection should be associative") = forAll: (a: Set[Numeral], b: Set[Numeral], c: Set[Numeral]) =>
    ((a ∩ b) ∩ c) == (a ∩ (b ∩ c))

  property("Union should be distributive over intersection") = forAll: (a: Set[Numeral], b: Set[Numeral], c: Set[Numeral]) =>
    (a ∪ (b ∩ c)) == ((a ∪ b) ∩ (a ∪ c))

  property("Intersection should be distributive over union") = forAll: (a: Set[Numeral], b: Set[Numeral], c: Set[Numeral]) =>
    (a ∩ (b ∪ c)) == ((a ∩ b) ∪ (a ∩ c))

  // Optional
  // Uncomment if needed
  property("Difference should be distributive over intersection") = forAll: (a: Set[Numeral], b: Set[Numeral], c: Set[Numeral]) =>
    (a \ (b ∩ c)) == ((a \ b) ∪ (a \ c))

end SetSpecification
