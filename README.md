# FarmFinder

The aim is to provide an android app which allows to search for certain groceries and find local farms which sell those from their yards. The backend was built for Windows Azure.

The farmers should be able to manage their groceries via a website. In addition, a SearchAPI (RESTful - serving JSON, XML and JSONP) was build separated from the website for scalability reasons. The website was build using WCF and Web API was used for the REST interface. Moreover, Lucene.NET is used as search engine. 

More information can be found in [doc/farmfinder.pdf](https://github.com/robstoll/farmfinder/blob/master/doc/farmfinder.pdf) (in German though).


Please be aware of that it is only a prototype and needs further work before it can be used in production. Moreover, I noticed that the class AzureDirectory does not seem to be production ready (see also the a corresponding entry on [stackoverflow](http://stackoverflow.com/a/18500738)).
---

Copyright 2015 Robert Stoll <rstoll@tutteli.ch>

Licensed under the Apache License, Version 2.0 (the "License");  
you may not use this file except in compliance with the License.  
You may obtain a copy of the License at  

[http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software  
distributed under the License is distributed on an "AS IS" BASIS,  
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  
See the License for the specific language governing permissions and  
limitations under the License.