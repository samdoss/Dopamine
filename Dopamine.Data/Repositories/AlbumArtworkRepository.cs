﻿using Digimezzo.Utilities.Log;
using Dopamine.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dopamine.Data.Repositories
{
    public class AlbumArtworkRepository : IAlbumArtworkRepository
    {
        private ISQLiteConnectionFactory factory;

        public AlbumArtworkRepository(ISQLiteConnectionFactory factory)
        {
            this.factory = factory;
        }

        public async Task DeleteAlbumArtworkAsync(string albumKey)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            conn.Execute($"DELETE FROM AlbumArtwork WHERE AlbumKey=?;", DataUtils.EscapeQuotes(albumKey));
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error("Could not delete AlbumArtwork. Exception: {0}", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not connect to the database. Exception: {0}", ex.Message);
                }
            });
        }

        public async Task<long> DeleteUnusedAlbumArtworkAsync()
        {
            long unusedAlbumArtworkCount = 0;

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            unusedAlbumArtworkCount = conn.ExecuteScalar<long>("SELECT COUNT(AlbumKey) FROM AlbumArtwork WHERE AlbumKey NOT IN (SELECT AlbumKey FROM Track);");
                            conn.Execute("DELETE FROM AlbumArtwork WHERE AlbumKey NOT IN (SELECT AlbumKey FROM Track);");
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error("Could not delete unused AlbumArtwork. Exception: {0}", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not connect to the database. Exception: {0}", ex.Message);
                }
            });

            return unusedAlbumArtworkCount;
        }

        public async Task<IList<AlbumArtwork>> GetAlbumArtworkAsync()
        {
            IList<AlbumArtwork> albumArtwork = new List<AlbumArtwork>();

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            albumArtwork = conn.Query<AlbumArtwork>("SELECT * FROM AlbumArtwork;");
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error("Could not get album artwork. Exception: {0}", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not connect to the database. Exception: {0}", ex.Message);
                }
            });

            return albumArtwork;
        }

        public async Task<IList<string>> GetArtworkIdsAsync()
        {
            IList<string> artworkIds = new List<string>();

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            artworkIds = conn.Query<AlbumArtwork>("SELECT * FROM AlbumArtwork;").Select(a => a.ArtworkID).ToList();
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error("Could not get artwork id's. Exception: {0}", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not connect to the database. Exception: {0}", ex.Message);
                }
            });

            return artworkIds;
        }
    }
}
