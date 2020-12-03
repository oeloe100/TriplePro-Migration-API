using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TPMHelper.AfostoHelper.ProductModel;

namespace TPMApi.Builder.Afosto
{
    public class CollectionsBuilder
    {
        public static void OriginalCollectionsBuilder(
            List<JObject> _wooCategoriesFromAfosto, 
            List<Collections> collectionsList, 
            int x)
        {
            List<Links> linkCollection = new List<Links>();
            foreach (var link in _wooCategoriesFromAfosto[x]["_links"])
            {
                Links links = new Links()
                {
                    Rel = link["rel"].ToString(),
                    Href = link["href"].ToString()
                };

                linkCollection.Add(links);
            }

            var collections = new Collections()
            {
                Id = (int)_wooCategoriesFromAfosto[x]["id"],
                Links = linkCollection
            };

            collectionsList.Add(collections);
        }

        public static void DefaultCollectionsBuilder(
            JArray defaultRequirements, 
            List<Collections> collectionsList)
        {
            List<Links> linkCollection = new List<Links>();
            foreach (var link in defaultRequirements[0]["_links"])
            {
                Links links = new Links()
                {
                    Rel = link["rel"].ToString(),
                    Href = link["href"].ToString()
                };

                linkCollection.Add(links);
            }

            var collections = new Collections()
            {
                Id = (int)defaultRequirements[0]["id"],
                Links = linkCollection
            };

            collectionsList.Add(collections);
        }

        public static void BundledCollectionsBuilder(
            JObject _wooCategoriesFromAfosto,
            List<Collections> collectionsList)
        {
            List<Links> linkCollection = new List<Links>();
            var link = _wooCategoriesFromAfosto["_links"][0];

            Links links = new Links()
            {
                Rel = link["rel"].ToString(),
                Href = link["href"].ToString()
            };

            linkCollection.Add(links);

            var collections = new Collections()
            {
                Id = (int)_wooCategoriesFromAfosto["id"],
                Links = linkCollection
            };

            collectionsList.Add(collections);
        }
    }
}
