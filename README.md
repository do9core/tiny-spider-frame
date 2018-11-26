# Tiny Spider Frame



## 简介（Introduction）

这是一个很简单的爬虫框架，主要用来爬取资源存到本地

(* This is a little spider frame. To download resources to local storage. *)

效率不高，单个顺序爬取

(* It is not very effective because default downloader download files one by one. *)

但是接口开放，支持独立实现

(* But the interfaces are open, you can handle them as you wish. *)



## 使用说明（Usage）

### 实现接口（Implement interfaces）

* `IFilenameParser`

  文件名提取接口，需要实现这个接口来处理保存的文件名，下面是一个例子

  (* This is the interface to get local file name from any object, you need to implement this interface.  Here is an example. *)

  ```csharp
  internal class MyFileNameParser : IFileNameParser
  {
  	//interface method
      public string GetFileName(object param)
      {
      	if(param is string url) {
      		return url.Split('/').Last();
      	}
      	
      	throw new ArgumentExcepetion(param);
      }
      
      //interface method
      public async Task<string> GetFileNameAsync(object param) 
      {
      	await Task.CompletedTask;
      	return GetFileName(param);
      }
  }
  ```

* `IFolderNameParser`

  目录名提取接口，需要实现这个接口来处理保存的目录名，下面是一个很简单的实现

  (* This is the interface to get local directory name from any object, you need to implement this interface, here is a very easy implementation. *)

  ```csharp
  internal class MyFolderNameParser : IFolderNameParser
  {
  	private const string FolderName = "MySpiderDownload";
  
  	//interface method
  	public string GetFolderName(object param)
      {
      	return FolderName;
      }
      
      //interface method
      public async Task<string> GetFolderNameAsync(object param)
      {
      	await Task.CompletedTask;
      	return GetFolderName(param);
      }
  }
  ```

* `ISourceParser`

  源提取接口，需要实现这个接口来指出需要下载的资源，下面有一个很简单的实现

  (* This is the interface to get download source, here is an example. *)

  ```csharp
  internal class MySourceParser : ISourceParser
  {
  	private static readonly Regex TargetRegex
  		= new Regex(@"<img id=""img"" src=""([Hh][Tt]{2}[Pp][Ss]?://.*?)"".*?>");
  		
  	private readonly HttpClient _client;
  	
  	public MySourceParser(HttpClient client)
      {
      	_client = client;
      }
      
      //interface method
      public IEnumerable<string> GetSources(object input)
      {
      	return GetSourcesAsync(input).Result;
      }
      
      //interface method
      public async Task<IEnumerable<string>> GetSourcesAsync(object input)
      {
      	if(input is string url)
          {
          	var html = await GetHtmlAsync(url);
          	return MatchSources(html);
          }
          
          throw new ArgumentException(input);
      }
      
      private async Task<string> GetHtmlAsync(string url)
      {
      	return await _client.GetStringAsync(url);
      }
      
      private async IEnumerable<string> MatchSources(string html)
      {
      	var matches = TargetRegex.Matches(html);
      	return from Match match in matches
      		   select match.Groups[1].Value;
      }
  }
  ```

* `IWaitTimer`

  暂停器接口，实现这个接口来让下载器进行暂停，避免被网站阻止访问，有事件指示开始暂停和结束暂停~~（没有什么用）~~

  (* This timer interface allow downloader to pause in some ways. Classes that implement this interface help you prevent from being barred. It has events to tell you when the pause begin and end. ~~Very useless!~~ *)

  ``` csharp
  internal class MyTimer : IWaitTimer
  {
  	private static readonly Random Sequence = new Random();
  
  	//interface events
  	public event WaitEventHandler OnWait;
  	public event WaitEventHandler Waited;
  	
  	//interface property
  	public bool Peek => Sequence.NextDouble() < 0.2;
  	
  	//interface method
  	public async Task Wait()
      {
      	await Task.Delay(Sequence.Next(2500,3000));
      }
  }
  ```

  你也可以用我制作的默认Timer——`DefaultWaitTimer`

  (* You can also use the `DefaultWaitTimer` *)

  ```csharp
  var timer = new DefaultWaitTimer();
  
  // set trigger limit
  timer.PeekLimit = 0.3;
  
  // set basic time
  timer.BaseTimeLimitDown = 300;
  timer.BaseTimeLimitUp = 500;
  
  // clear default pairs
  timer.FactorLimits.Clear();
  
  // use ValueTuple
  timer.FactorLimits.Add((10, 25));
  timer.FactorLimits.Add((100, 125));
  
  // set behavior
  timer.FactorOperation = FactorOperation.Product;
  
  // then you have 30% to get a pause time:
  // rand(300, 500) + (rand(10, 25) * rand(100, 125))
  ```

* `IDownloader`

  下载器接口，默认实现的是一个顺序下载器，你也可以通过实现这个接口来实现更高效的下载，和timer类似，此接口带有开始下载和结束下载事件，默认实现是`DefaultDownloader`

  (* The downloader interface, default downloader downloads item one by one. You can implement this interface to create a effective downloader. Like the timer, this interface has events to let you know when download start and end. Default downloader class is `DefaultDownlader` *)

### 编写爬虫（Extend spider base）

* `SpiderBase`

  通过扩展这个类来使用本框架，需要实现的接口前面已经说清了，下面直接举一个例子

  ```csharp
  internal class MySpider : SpiderBase
  {
  	public MySpider(HttpClient client)
  		: base(client,
  				new DefaultDownloader(client, new MyFileNameParser())),
  				new MySourceParser(client),
  				new MyFolderNameParser()) { }
  				
  	/* There are two hooks to help the interfaces: */
  	 
  	/* 1. object GetSourceParserArgument(object input)
  	 *
  	 * This one get argument for the ISourceParser
  	 * if you need to do something 
  	 * before the source parser works with input, 
  	 * you should override this.
  	*/	
  	
  	/* 2. object GetFolderNameParserArgument(object input);
  	 *
  	 * this one get argument for the IFolderNameParser
  	 * if you need to do something 
  	 * before the folder name parser works with input, 
  	 * you should override this.
  	 */
  }
  ```

  实现这个类后，就可以使用这个爬虫了

  ```csharp
  public class Demo
  {
      public static async void Run()
      {
          var client = new HttpClient();
          var spider = new MySpider(client);
          
          spider.DownloadComplete += 
              (e) => Console.WriteLine("Download Finished!");
          
          for(;;)
          {
              Console.WriteLine("Url>");
              var url = Console.ReadLine();
              
              await spider.DownloadFrom(url);
          }
      }
  }
  ```


## 你为啥还不赶快动手呢？（Why not have a try?）

恭喜你看完啦！

(* Thanks for reading! *)