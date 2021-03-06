// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

// We don't want to run the tests in parallel so we don't collide store state.
// If we add the identity based constructors we could potentially
// create unique identities for every test to allow every test to have
// it's own store.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]
[assembly: SkipOnMono("System.IO.IsolatedStorage is not supported on Browser", TestPlatforms.Browser)]
