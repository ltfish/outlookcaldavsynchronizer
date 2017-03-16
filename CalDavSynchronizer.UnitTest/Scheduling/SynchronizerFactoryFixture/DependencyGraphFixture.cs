// This file is Part of CalDavSynchronizer (http://outlookcaldavsynchronizer.sourceforge.net/)
// Copyright (c) 2015 Gerhard Zehetbauer
// Copyright (c) 2015 Alexander Nimmervoll
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.using System;

using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CalDavSynchronizer.Contracts;
using CalDavSynchronizer.Implementation;
using CalDavSynchronizer.Implementation.TimeZones;
using CalDavSynchronizer.Scheduling;
using GenSync.ProgressReport;
using NUnit.Framework;

namespace CalDavSynchronizer.UnitTest.Scheduling.SynchronizerFactoryFixture
{
  public class DependencyGraphFixture
  {
    private SynchronizerFactory _synchronizerFactory;

    [SetUp]
    public void SetUp()
    {
      _synchronizerFactory = new SynchronizerFactory(
        g => @"A:\data",
        NullTotalProgressFactory.Instance,
        new OutlookSessionStub(),
        new NullDaslFilterProvider(),
        NullOutlookAccountPasswordProvider.Instance,
        new GlobalTimeZoneCache(),
        new NullQueryOutlookFolderStrategy());
    }

    [Test]
    public async Task Test_ContactSynchronizerWithSogoDistLists ()
    {
      var synchronizer = await _synchronizerFactory.CreateSynchronizer(
        new Options
        {
          CalenderUrl = "http://server",
          ConflictResolution = ConflictResolution.Automatic,
          IgnoreSynchronizationTimeRange = true,
          IsChunkedSynchronizationEnabled = false,
          MappingConfiguration = new ContactMappingConfiguration
          {
            MapDistributionLists = true,
            DistributionListType = DistributionListType.Sogo
          },
          SynchronizationMode = SynchronizationMode.MergeInBothDirections,
          OutlookFolderEntryId = "eid",
          OutlookFolderStoreId = "sid",
          ServerAdapterType = ServerAdapterType.WebDavHttpClientBased
        },
        new GeneralOptions());

      CheckGraph(synchronizer, "ContactSynchronizerWithSogoDistLists.txt");
      
    }

    [Test]
    public async Task Test_ContactSynchronizerWithVCardGroupDistLists ()
    {
      var synchronizer = await _synchronizerFactory.CreateSynchronizer (
        new Options
        {
          CalenderUrl = "http://server",
          ConflictResolution = ConflictResolution.Automatic,
          IgnoreSynchronizationTimeRange = true,
          IsChunkedSynchronizationEnabled = false,
          MappingConfiguration = new ContactMappingConfiguration
          {
            MapDistributionLists = true,
            DistributionListType = DistributionListType.VCardGroup
          },
          SynchronizationMode = SynchronizationMode.MergeInBothDirections,
          OutlookFolderEntryId = "eid",
          OutlookFolderStoreId = "sid",
          ServerAdapterType = ServerAdapterType.WebDavHttpClientBased
        },
        new GeneralOptions ());

      CheckGraph (synchronizer, "ContactSynchronizerWithVCardGroupDistLists.txt");

    }


    [Test]
    public async Task Test_ContactSynchronizerWithoutDistLists ()
    {
      var synchronizer = await _synchronizerFactory.CreateSynchronizer (
        new Options
        {
          CalenderUrl = "http://server",
          ConflictResolution = ConflictResolution.Automatic,
          IgnoreSynchronizationTimeRange = true,
          IsChunkedSynchronizationEnabled = false,
          MappingConfiguration = new ContactMappingConfiguration
          {
            MapDistributionLists = false,
          },
          SynchronizationMode = SynchronizationMode.MergeInBothDirections,
          OutlookFolderEntryId = "eid",
          OutlookFolderStoreId = "sid",
          ServerAdapterType = ServerAdapterType.WebDavHttpClientBased
        },
        new GeneralOptions ());

      CheckGraph (synchronizer, "ContactSynchronizerWithoutDistLists.txt");
    }

    private void CheckGraph(object o, string graphDefinition)
    {
      var graph = CreateGraph(o);

      Assert.That(
        graph,
        Is.EqualTo(GetGraphDefinition(graphDefinition)));
    }

    private static string CreateGraph(object o)
    {
      var typeWithDependecies = TypeWithDependecies.GetTypeWithDependecies(o);
      var stringBuilder = new StringBuilder();
      typeWithDependecies.ToString(stringBuilder);
      var graph = stringBuilder.ToString();
      return graph;
    }

    private string GetGraphDefinition(string name)
    {
      using (var s = Assembly.GetExecutingAssembly ().GetManifestResourceStream ($"{typeof (DependencyGraphFixture).Namespace}.DependencyGraphs.{name}"))
      using (var reader = new StreamReader(s))
        return reader.ReadToEnd();
    }

    private void SaveGraph(object o, string graphDefinition)
    {
      File.WriteAllText(
        @"D:\dev\CalDavSynchronizer\CalDavSynchronizer.UnitTest\Scheduling\SynchronizerFactoryFixture\DependencyGraphs\" + graphDefinition,
        CreateGraph(o));
    }
  }
}