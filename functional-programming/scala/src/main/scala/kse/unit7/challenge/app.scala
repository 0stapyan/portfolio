package kse.unit7.challenge

import kse.unit7.challenge.adt.*
import kse.unit7.challenge.model.*
import kse.unit7.challenge.services.*

object app:

  def getPostsViews(apiKey: ApiKey): Try[List[PostView]] =
    for
      userProfile <- services.getUserProfile(apiKey)
      posts       <- services.getPosts(userProfile.userId)
      views <- posts.foldLeft(Try(List.empty[PostView])) { (accTry, post) =>
        for
          acc  <- accTry
          view <- getPostView(post)
        yield acc :+ view
      }
    yield views

  def getPostsViewDesugared(apiKey: ApiKey): Try[List[PostView]] =
    services.getUserProfile(apiKey).flatMap { userProfile =>
      services
        .getPosts(userProfile.userId)
        .map { posts =>
          posts.foldLeft(Try(List.empty[PostView])) { (accTry, post) =>
            accTry.flatMap { acc =>
              getPostViewDesugared(post).map { view =>
                acc :+ view
              }
            }
          }
        }
        .flatMap(identity)
    }

  def getPostView(post: Post): Try[PostView] =
    for
      comments <- services.getComments(post.postId)
      likes    <- services.getLikes(post.postId)
      shares   <- services.getShares(post.postId)
    yield PostView(post, comments, likes, shares)

  def getPostViewDesugared(post: Post): Try[PostView] =
    services.getComments(post.postId).flatMap { comments =>
      services.getLikes(post.postId).flatMap { likes =>
        services.getShares(post.postId).map { shares =>
          PostView(post, comments, likes, shares)
        }
      }
    }
