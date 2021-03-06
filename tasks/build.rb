module BasicUnity

  class Build < Thor
    namespace :build
    include Thor::Actions
    include BasicUnity::BasicUnityHelper

    # adds :quiet, :skip, :pretent, :force
    add_runtime_options!

    class_option :development, :type => :boolean, :aliases => "-d", :desc => "Set the development or production stage"
    class_option "defines", :type => :string, :desc => "comma delimited string of compiler defines"
    class_option "simulator", :type => :boolean, :desc => "target iOS/tvOS simulator (defaults to actual device)"

    desc "all", "build for Windows, Linux and OSX using Unity batch mode"
    def all
      say_status "Windows", content, :green
      result = build_target("Windows")
      if result
        say_status "Linux", content, :green
        result = build_target("Linux")
      end
      if result
        say_status "macOS", content, :green
        result = build_target("OSX")
      end
    end

    desc "osx", "build for OSX using Unity batch mode"
    def osx
      build_target("OSX")
    end

    desc "windows", "build for Windows using Unity batch mode"
    def windows
      build_target("Windows")
    end

    desc "linux", "build for Linux using Unity batch mode"
    def linux
      build_target("Linux")
    end

    desc "ios", "build for iOS using Unity batch mode"
    def ios
      build_target("IOS")
    end

    desc "tvos", "build for tvOS using Unity batch mode"
    def tvos
      build_target("TvOS")
    end

    desc "android", "build for Android using Unity batch mode"
    def android
      build_target("Android")
    end

    private

    # where to start looking for templates, required by the template methods
    # even though we are using absolute paths
    def self.source_root
      File.dirname(__FILE__)
    end

    def set_instance_variables
      @simulator = options[:simulator]
      @stage = options[:development] ? "development" : default_stage
    end

    # http://docs.unity3d.com/Documentation/Manual/CommandLineArguments.html
    # logfile w/o params will go to STDOUT
    # NOTE: Throw exception in Unity to exit with non-zero return
    def build_command(target)
      build = @stage == "development" ? "Development" : "Release"
      opt = []
      opt << "--defines \"#{options[:defines]}\"" if options[:defines]
      "#{unity_binary} -batchmode -nographics -logFile -quit -executeMethod Jammer.Editor.Builder.Perform#{target}#{build}Build #{opt.join(' ')}"
    end

    # do the build, return true on success
    def build_target(target)
      set_instance_variables
      assert_working_folder(ROOT_FOLDER)

      if (target == "IOS")
        if @simulator
          # simulator sdk
          write_ios_sdk("989")
        else
          # default to device sdk
          write_ios_sdk("988")
        end
      end

      # invoke with "new" to make sure the task can run multiple time
      Build.new.invoke("defines:save")
      begin
        write_version_build(nil)
        command = build_command(target)
        logfile = build_logfile_filename(target)
        result = run_command(command, logfile)
      ensure
        # invoke with "new" to make sure the task can run multiple time
        Build.new.invoke("defines:restore")
      end

      # grep and display errors
      cmd = "grep 'Error building Player' #{logfile}"
      grep_result = system cmd
      if grep_result
        say_status "BUILD ERROR", "review logs for specific error", :red
      end

      (result.exitstatus == 0) && (!grep_result)
    end

  end
end
