require 'pathname'
require 'rbconfig'
require 'fileutils'
require 'json'
require 'neatjson'


module BasicUnity

  ROOT_FOLDER = File.expand_path(File.join(File.dirname(__FILE__), ".."))
  ASSETS_FOLDER = File.join(ROOT_FOLDER, "Assets")
  RESOURCES_FOLDER = File.join(ASSETS_FOLDER, "Resources")
  DOC_FOLDER = File.join(ROOT_FOLDER, "doc")
  PROJECT_FOLDER = File.join(ROOT_FOLDER, "ProjectSettings")
  VENDOR_FOLDER = File.join(ASSETS_FOLDER, "Plugins", "Vendor")
  TMP_FOLDER = File.join(ROOT_FOLDER, "tmp")
  BUILD_FOLDER = File.join(ROOT_FOLDER, "build")
  PKG_FOLDER = File.join(ROOT_FOLDER, 'pkg')
  STAGING_FOLDER = File.join(TMP_FOLDER, 'staging')
  TASKS_FOLDER = File.join(ROOT_FOLDER, 'tasks')
  LIBRARY_FOLDER = File.join(ROOT_FOLDER, 'Library')

  PRODUCT_IDENTIFIER_BASE = "com.example"
  PRODUCT_IDENTIFIER_DEFAULT = "com.example.jammer"

  JSON_OPTIONS = { :aligned => true, :around_colon => 1 }

  module BasicUnityHelper

    # ensure work directory or throw error
    def assert_working_folder(folder)
      throw "You are in the wrong folder. Expected PWD == #{folder}" unless (folder == File.expand_path(Dir.pwd))
    end

    # return 1 if any character in the input string is upper case
    def case_insensitive(input)
      return 1 unless input

      input.split("").each do |i|
        return 0 if /[[:upper:]]/ =~ i
      end

      return 1
    end

    # @return [Symbol] OS specific ID
    def os
      @os ||= (

        require "rbconfig"
        host_os = RbConfig::CONFIG['host_os'].downcase

        case host_os
        when /linux/
          :linux
        when /darwin|mac os/
          :mac
        when /mswin|msys|mingw32/
          :windows
        when /cygwin/
          :cygwin
        when /solaris/
          :solaris
        when /bsd/
          :bsd
        else
          raise Error, "unknown os: #{host_os.inspect}"
        end
      )
    end

    # @return [Boolean] true if POSIX system
    def posix?
      !windows?
    end

    # @return [Boolean] true if JRuby platform
    def jruby?
      platform == :jruby
    end

    # @return [Boolean] true if Mac OSX
    def mac?
      os == :mac
    end

    # @return [Boolean] true if any version of Windows
    def windows?
      os == :windows
    end

    # @return [Symbol] OS symbol or :jruby if java platform
    def platform
      if RUBY_PLATFORM == "java"
        :jruby
      else
        os
      end
    end

    # @return [String] the default stage
    def default_stage
      "production"
    end

    # @return [String] the product name, override Unity with a ".product" file
    def default_product
      filename = File.join(ROOT_FOLDER, ".product")
      contents = nil

      if File.exists?(filename)
        File.open(filename, "r") do |f|
          contents = f.read.strip
        end
      end

      return contents.nil? ? read_product_name : contents
    end

    def unity_binary
      binary_folder = unity_binary_folder

      if mac?
        "/Applications/#{binary_folder}/Unity.app/Contents/MacOS/Unity"
      else
        # 'start /wait' facilitates getting the proper result code
        # https://bitbucket.org/Unity-Technologies/unitytesttools/wiki/UnitTestsRunner

        # CMD.exe may have trouble with spaces, use 8.3 version with windows slashes
        binary = "c:\\PROGRA~1\\#{binary_folder}\\Editor\\Unity.exe"

        # check d drive
        if !File.exists?(binary)
          binary = "c:\\bin\\#{binary_folder}\\Editor\\Unity.exe"
        end

        "start /WAIT #{binary}"
      end
    end

    def shell_quote(string)
      return "" if string.nil? or string.empty?
      if windows?
        %{"#{string}"}
      else
        string.split("'").map{|m| "'#{m}'" }.join("\\'")
      end
    end

    # @return[String] the relative path from the CWD
    def relative_path(path)
      return unless path

      path = Pathname.new(File.expand_path(path, FileUtils.pwd))
      cwd = Pathname.new(FileUtils.pwd)

      if windows?
        # c:/home D:/path/here will faile with ArgumentError: different prefix
        return path.to_s if path.to_s.capitalize[0] != cwd.to_s.capitalize[0]
      end

      path = path.relative_path_from(cwd)
      path = "./#{path}" unless path.absolute? || path.to_s.match(/^\./)
      path.to_s
    end

    # execute a block inside a folder
    def in_path(path, &block)
      if path
        Dir.chdir(path, &block)
      else
        block.call
      end
    end

    # @return [String] company full name from project settings
    def read_company_name
      version_info_file = File.join(PROJECT_FOLDER, "ProjectSettings.asset")
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        contents.match(/companyName: (.*)$/)
        $1
      end
    end

    # @return [String] product full name from project settings
    def read_product_name
      version_info_file = File.join(PROJECT_FOLDER, "ProjectSettings.asset")
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        contents.match(/productName: (.*)$/)
        $1
      end
    end

    # @return [String] product indentifier from project settings
    def read_product_identifier
      version_info_file = File.join(PROJECT_FOLDER, "ProjectSettings.asset")
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        contents.match(/bundleIdentifier: (.*)$/)
        $1
      end
    end

    # @return [String] the version in #.#.# format
    def read_version_number(version_info_file=nil)
      version_info_file = File.join(ASSETS_FOLDER, "Resources", "Version.txt") unless version_info_file
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        json = JSON.parse(contents)
        json["version"]
      end
    end

    # @return [String] the code in # format
    def read_version_code(version_info_file=nil)
      version_info_file = File.join(ASSETS_FOLDER, "Resources", "Version.txt") unless version_info_file
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        json = JSON.parse(contents)
        json["code"]
      end
    end

    # @return [String] the build in # format
    def read_version_build(version_info_file=nil)
      version_info_file = File.join(ASSETS_FOLDER, "Resources", "Version.txt") unless version_info_file
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        json = JSON.parse(contents)
        json["build"]
      end
    end

    # @return [String] the unityVersion
    def read_unity_version(version_info_file=nil)
      version_info_file = File.join(ASSETS_FOLDER, "Resources", "Version.txt") unless version_info_file
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        json = JSON.parse(contents)
        json["unity"]
      end
    end

    # @return [String] the entire version formatted string
    def read_version_string(version_info_file=nil)
      version_info_file = File.join(ASSETS_FOLDER, "Resources", "Version.txt") unless version_info_file
      "#{read_version_number(version_info_file)}.#{read_version_code(version_info_file)}.#{read_version_build(version_info_file)}"
    end

    # write version number directly to Version.txt
    def write_version_number(number, version_info_file=nil)
      version_info_file = File.join(ASSETS_FOLDER, "Resources", "Version.txt") unless version_info_file

      json = ""
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        json = JSON.parse(contents)
      end

      json["version"] = number
      File.open(version_info_file, 'w') { |file| file.write(JSON.neat_generate(json, JSON_OPTIONS)) }
    end

    # write version number directly to ProjectSettings.asset
    def write_project_number(number, version_info_file=nil)
      version_info_file = File.join(PROJECT_FOLDER, "ProjectSettings.asset") unless version_info_file
      contents = ""
      File.open(version_info_file, "r") do |f|
        contents = f.read
      end

      contents = contents.gsub(/bundleVersion: [\d]+\.[\d]+\.[\d]+$/, "bundleVersion: #{number}")
      File.open(version_info_file, 'w') { |file| file.write(contents) }
    end

    # write build code number directly to Version.txt
    def write_version_code(code, version_info_file=nil)
      version_info_file = File.join(ASSETS_FOLDER, "Resources", "Version.txt") unless version_info_file

      json = ""
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        json = JSON.parse(contents)
      end

      json["code"] = code
      File.open(version_info_file, 'w') { |file| file.write(JSON.neat_generate(json, JSON_OPTIONS)) }
    end

    # write build code number directly to ProjectSettings.asset
    def write_project_code(code)
      version_info_file = File.join(PROJECT_FOLDER, "ProjectSettings.asset")
      contents = ""
      File.open(version_info_file, "r") do |f|
        contents = f.read
      end

      # Android
      contents = contents.gsub(/AndroidBundleVersionCode: [\d]+$/, "AndroidBundleVersionCode: #{code}")

      # iOS
      contents = contents.gsub(/iPhoneBuildNumber: [\d]+$/, "iPhoneBuildNumber: #{code}")
      File.open(version_info_file, 'w') { |file| file.write(contents) }
    end

    # write build build number directly to Version.txt
    def write_version_build(sha, version_info_file=nil)
      sha = generate_build_number unless sha
      version_info_file = File.join(ASSETS_FOLDER, "Resources", "Version.txt") unless version_info_file

      json = ""
      File.open(version_info_file, "r") do |f|
        contents = f.read.strip
        json = JSON.parse(contents)
      end

      json["build"] = sha
      File.open(version_info_file, 'w') { |file| file.write(JSON.neat_generate(json, JSON_OPTIONS)) }
    end

    def generate_build_number
      `git log --pretty=format:%h --abbrev-commit -1`
    end

    # @return [String] the default unity binary folder name
    def unity_binary_folder
      filename = File.join(ROOT_FOLDER, ".unity")
      contents = nil

      if File.exists?(filename)
        File.open(filename, "r") do |f|
          contents = f.read.strip
        end
      end

      return contents.nil? ? "Unity" : contents
    end

    # run using thor, this is the most compatible run method
    # returns global $? for exit status, cannot capture output
    def run_command(command, logfile=nil)

      if logfile
        # backticks to capture output
        say_status "run", command, :green
        output = `#{command}`
        File.open(logfile, 'w') do |file|
          file.write(output)
        end
      else
        # normal thor run shows stdout but can't capture it
        run(command)
      end

      if ($?.exitstatus == 0)
        say $?.inspect, :green
        say
        say_status "command succeeded", "", :green
      else
        say $?.inspect, :yellow
        say
        say_status "command failed", "", :red
      end

      $?
    end

  end
end


