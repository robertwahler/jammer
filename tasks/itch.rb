module BasicUnity

  class Itch < Thor
    namespace :itch
    include Thor::Actions
    include BasicUnity::BasicUnityHelper

    # adds :quiet, :skip, :pretent, :force
    add_runtime_options!

    class_option :version, :type => :string, :desc => "Override the default version number"
    class_option :package, :type => :string, :desc => "Override the default name of the package archive to transfer"
    class_option :development, :type => :boolean, :aliases => "-d", :desc => "Development options, includes debug symbols, etc"
    class_option "dry-run", :type => :boolean, :desc => "Test run, do everything but run the butler command"

    desc "login", "Grant Itch.io's butler access to your Itch account. Needs to be run just once per machine install. Nees your itch_id in this script"
    def login
      run "'#{butler_binary}' login"
    end

    desc "linux", "Push current package archive to Itch.io for Linux"
    def linux
      set_push_instance_variables("linux", "linux-universal")
      execute @cmd
    end

    desc "windows", "Push current package archive to Itch.io for Windows"
    def windows
      set_push_instance_variables("windows", "windows")
      execute @cmd
    end

    desc "osx", "Push current package archive to Itch.io for macOS"
    def osx
      set_push_instance_variables("osx", "osx-universal")
      execute @cmd
    end

    desc "status", "Get the status of the current product"
    def status
      execute "'#{butler_binary}' status #{itch_id}/#{default_product} --verbose"
    end

    private

    def butler_binary
      if mac?
        File.expand_path("~/Library/Application\ Support/itch/bin/butler")
      else
        "butler.exe"
      end
    end

    def execute(cmd)
      if @dry_run
        say_status "Dry run", cmd, :white
      else
        run cmd
      end

    end

    # where to start looking for templates, required by the template methods
    # even though we are using absolute paths
    def self.source_root
      File.dirname(__FILE__)
    end

    def itch_id
      "INSERT_YOUR_PUBLIC_ITCH_ID_HERE"
    end

    def set_push_instance_variables(platform, channel)
      @product = default_product
      @platform = platform
      @dry_run = options["dry-run"]
      @version = options[:version] ? options[:version] : read_version_string
      # itch.io's butler only supports zip files at this time. See https://github.com/itchio/butler/issues/47
      @extension = (@platform == "linux") ? "zip" : "zip"
      @package = options[:package] ? options[:package] : "#{PKG_FOLDER}/#{@product}.#{@version}.#{@platform}.#{@extension}"
      # channel names control initial tags but they can be permanently changed later
      # https://itch.io/docs/butler/pushing.html#channel-names
      @channel = channel
      @cmd = "'#{butler_binary}' push #{@package} #{itch_id}/#{@product}:#{@channel} --userversion #{@version}"
    end

  end
end


