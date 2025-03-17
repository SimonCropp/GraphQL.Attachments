import {Apollo, APOLLO_OPTIONS} from 'apollo-angular'
import {HttpLink} from 'apollo-angular/http'
import {print, stripIgnoredCharacters} from 'graphql'
import {ApplicationConfig, inject} from '@angular/core'
import {
  ApolloClientOptions,
  ApolloLink,
  InMemoryCache,
  NormalizedCacheObject,
  Operation
} from '@apollo/client/core'
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore
import extractFiles from 'extract-files/extractFiles.mjs'
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore
import isExtractableFile from 'extract-files/isExtractableFile.mjs'

const uri = 'https://localhost:7233/graphql'

// begin-snippet: SetupApolloProvider
export function apolloOptionsFactory(): ApolloClientOptions<NormalizedCacheObject> {
  const httpLink = inject(HttpLink)
  const link = httpLink.create({
    uri: uri,
    useGETForQueries: true,
    operationPrinter: (operation) => stripIgnoredCharacters(print(operation)),
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    // apollo client requires extract files to create proper form data for post
    extractFiles: (body: unknown) => {
      return extractFiles(body, isExtractableFile)
    }
  })
  return {
    link: ApolloLink.from([link]),
    cache: new InMemoryCache()
  }
}

export const graphqlProvider: ApplicationConfig['providers'] = [
  Apollo,
  {provide: APOLLO_OPTIONS, useFactory: apolloOptionsFactory}
]
// end-snippet
