{BufferedProcess} = require("atom")

module.exports =
  process: null
  popup: null
  view: null
  messages: null
  log: null

  show: (message) ->
    message = JSON.parse(message)

    unless @popup
        @popup = atom.notifications.addInfo("WebSharper log", dismissable: true, detail: message.text)
        @view = $(atom.views.getView(@popup))
        @messages = @view.find(".detail-content")
    else
        if message.details?.url
            ending =  " at <span class=\"inline-block highlight-success\">"
            ending += "<a href=\"" + message.details.url + "\">" + message.details.url + "</a>"
            ending += "</span>..."

            message.text = message.text.replace("...", ending)

            @view.removeClass("info").addClass("success")

        line = "<div class=\"line\">"

        if message.file
            line += "<span class=\"inline-block highlight-" + message.type + "\">" + message.file + "</span>"
        if message.location
            line += "<span>(line: " + message.location.line + ", column: " + message.location.column + ")</span>"

        @messages.append(line + "<div>" + message.text + "</div></div>")

    if message.type != "info" and @view.hasClass("info")
        @view.removeClass("info").addClass(message.type)

  activate: () ->
    atom.commands.add("atom-workspace", "sitelet:run", => @run())
    atom.commands.add("atom-workspace", "sitelet:stop", => @stop())

    @log = (message) =>
      messages = message.split(/\r?\n/).filter((message) -> message.length > 0)

      @show message for message in messages

  run: ->
    paths = atom.project.getPaths()
    packagePath = atom.packages.resolvePackagePath("ionide-websharper")

    if paths.length > 0
        @stop()

        @process = new BufferedProcess({
            command: packagePath + "\\binaries\\WebSharper.Atom",
            args: paths,
            stdout: @log,
            stderr: (x) -> console.error(x)
        })
    else
        atom.notifications.addError("WebSharper log", detail: "Can not find any F# project.")

  stop: ->
    if @process
        @process.kill()
        @process = null

    if @popup
        @popup.dismiss()
        @popup = null
