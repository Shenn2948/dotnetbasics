﻿using System.Collections.Generic;
using CsvParserApp.CsvModels;
using CsvParserApp.Models;

namespace CsvParserApp
{
    public interface IMovieDataSetBuilder
    {
        void AddMovies(IEnumerable<MovieRow> movies);

        void AddRatings(IEnumerable<RatingRow> ratings);

        void AddTags(IEnumerable<TagRow> tags);

        MovieDataSet Build();
    }
}
