package kse.unit7.challenge

object adt:

  enum Try[+V]:
    case Success(value: V)
    case Failure(exception: Throwable)

    def flatMap[Q](f: V => Try[Q]): Try[Q] = this match
      case Success(v) =>
        scala.util.Try(f(v)) match
          case scala.util.Success(result) => result
          case scala.util.Failure(ex)     => Failure(ex)
      case Failure(e) => Failure(e)

    def map[Q](f: V => Q): Try[Q] = this match
      case Success(v) =>
        scala.util.Try(f(v)) match
          case scala.util.Success(result) => Success(result)
          case scala.util.Failure(ex)     => Failure(ex)
      case Failure(e) => Failure(e)

  object Try:

    def apply[V](v: => V): Try[V] =
      scala.util.Try(v) match
        case scala.util.Success(result) => Success(result)
        case scala.util.Failure(ex)     => Failure(ex)
