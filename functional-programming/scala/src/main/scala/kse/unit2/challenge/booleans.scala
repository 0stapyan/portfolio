package kse.unit2.challenge

import scala.annotation.{tailrec, targetName}

object booleans:

  case object True
  case object False

  type True    = True.type
  type False   = False.type
  type Boolean = True | False

  val negation: Boolean => Boolean =
    case True  => False
    case False => True

  val conjunction: (Boolean, => Boolean) => Boolean = (a, b) =>
    a match
      case False => False
      case True  => b

  val disjunction: (Boolean, => Boolean) => Boolean = (a, b) =>
    a match
      case True  => True
      case False => b

  val implication: (Boolean, => Boolean) => Boolean = (a, b) => disjunction(negation(a), b)

  val equivalence: (Boolean, => Boolean) => Boolean = (a, b) => conjunction(implication(a, b), implication(b, a))

  extension (value: Boolean)

    @targetName("negation")
    infix def unary_! : Boolean = negation(value)

    @targetName("conjunction")
    infix def ∧(that: => Boolean): Boolean = conjunction(value, that)

    @targetName("disjunction")
    infix def ∨(that: => Boolean): Boolean = disjunction(value, that)

    @targetName("implication")
    infix def →(that: => Boolean): Boolean = implication(value, that)

    @targetName("equivalence")
    infix def ↔(that: => Boolean): Boolean = equivalence(value, that)

  def fold(operation: (Boolean, => Boolean) => Boolean, unit: Boolean)(list: List[Boolean]): Boolean =
    @tailrec
    def foldRec(lst: List[Boolean], acc: Boolean): Boolean = lst match
      case Nil          => acc
      case head :: tail => foldRec(tail, operation(head, acc))

    foldRec(list, unit)

  val conjunctionOfElements: List[Boolean] => Boolean = fold(conjunction, True)
  val disjunctionOfElements: List[Boolean] => Boolean = fold(disjunction, False)

  extension (booleans: List[Boolean])
    infix def conjunction: Boolean = conjunctionOfElements(booleans)
    infix def disjunction: Boolean = disjunctionOfElements(booleans)

end booleans
