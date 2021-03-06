﻿using dotNetCoreAPI.Dtos;
using dotNetCoreAPI.Model;
using dotNetCoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotNetCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewersController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private  IReviewRepository _reviewRepository;
        public ReviewersController(IReviewerRepository reviewerRepository,IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }

        //api/reviewers
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _reviewerRepository.GetReviewers();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewersDto = new List<ReviewerDto>();
            foreach (var reviewer in reviewers)
            {
                reviewersDto.Add(new ReviewerDto
                {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName= reviewer.LastName
                });
            }
            return Ok(reviewersDto);
        }


        //api/reviewers/id
        [HttpGet("{reviewerId}", Name="GetReviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
                {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName = reviewer.LastName
                };
            
            return Ok(reviewerDto);
        }

        // To GetAllReviewsOfAReviewer
        //api/review/reviewerId/reviews
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetAllReviewsOfAReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviews = _reviewerRepository.GetAllReviewsOfAReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var  review in reviews)
            {
                reviewsDto.Add(new ReviewDto()
                {
                    Id = review.Id,
                    Headline = review.Headline,
                    Rating =review.Rating,
                    ReviewText = review.ReviewText
                });
            }

            return Ok(reviewsDto);
        }
       
        // To Get Reviewer of a review
        //api/reviewers/reviewId/reviewer
        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewer(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

        //api/reviewers
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody]Reviewer reviewerToCreate)
        {
            if (reviewerToCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.CreateReviewer(reviewerToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {reviewerToCreate.FirstName} {reviewerToCreate.LastName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReviewer", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate);
        }



        //api/reviewers/reviewerid
        [HttpPut("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(204)]//No Content

        public IActionResult UpdateReviewer(int reviewerId, [FromBody]Reviewer reviewerToUpdate)
        {
            if (reviewerToUpdate == null)
                return BadRequest(ModelState);

            if (reviewerId != reviewerToUpdate.Id)
                return BadRequest(ModelState);

            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.UpdateReviewer(reviewerToUpdate))
            {
                ModelState.AddModelError("", $"Something went wrong updating reviewer");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        //api/reviewers/reviewerid
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(204)]//No Content

        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);
            var reviewsToDelete = _reviewerRepository.GetAllReviewsOfAReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", $"Somethong went wrong deleting {reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
                return StatusCode(500, $"Somethong went wrong deleting {reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
            }

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Somethong went wrong deleting reviews by {reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
                return StatusCode(500, $"Somethong went wrong deleting reviews by {reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
            }

            return NoContent();
        }

    }
}
